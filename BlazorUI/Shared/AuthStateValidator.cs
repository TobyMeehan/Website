using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorUI.Shared
{
    public class AuthStateValidator : ComponentBase
    {
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private IRoleProcessor roleProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }

        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Run(() =>
            {
                var timer = new Timer(5000);
                timer.Elapsed += async (source, e) =>
                {
                    if (_context.User.Identity.IsAuthenticated)
                    {
                        User user = mapper.Map<User>(await userProcessor.GetUserById(_context.User.GetUserId()));
                        if (!await StateIsValid(_context.User, user))
                        {
                            navigationManager.NavigateTo($"/login?redirectUri={navigationManager.Uri}", true);
                        }
                    }
                };
                timer.AutoReset = true;
                timer.Enabled = true;
            });
        }

        private async Task<bool> StateIsValid(ClaimsPrincipal authenticatedUser, User user)
        {
            // Because MS is difficult we have to extract a list of roles with much difficulty from the principal
            var roles = new List<Role>();

            foreach (var claim in authenticatedUser.Claims.Where(c => c.Type == ClaimTypes.Role))
            {
                Role role = await Task.Run(async () => mapper.Map<Role>(await roleProcessor.GetRoleByName(claim.Value)));
                roles.Add(role);
            }

            bool valid = true;

            // For the state to be valid, username and roles must match
            // TODO: Add more state validation if it is required

            valid = (authenticatedUser.GetUsername() == user.Username) && valid; // check usernames are the same

            // If each role in each list has a match and both lists are the same length, they must be equal
            valid = (user.Roles.Count == roles.Count) && valid;

            foreach (Role role in roles)
            {
                valid = user.Roles.Any(x => role.Id == x.Id) && valid;
            }

            return valid;
        }
    }
}
