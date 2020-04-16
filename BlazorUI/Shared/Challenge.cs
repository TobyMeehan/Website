using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Shared
{
    public class Challenge : ComponentBase
    {
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private AlertState alertState { get; set; }

        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();

            if (!_context.User.Identity.IsAuthenticated)
            {
                string uri = $"/login?ReturnUrl={System.Net.WebUtility.UrlEncode(navigationManager.ToBaseRelativePath(navigationManager.BaseUri))}";
                navigationManager.NavigateTo(uri, true);
            }
            else
            {
                alertState.Queue.Add(new Models.Alert
                {
                    Title = "Hey!",
                    Body = "You were not authorised to do whatever you just tried to do.",
                    Context = AlertContext.Danger
                });

                navigationManager.NavigateTo("/");
            }
        }
    }
}
