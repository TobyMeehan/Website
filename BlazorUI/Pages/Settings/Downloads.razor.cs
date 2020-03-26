using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Settings
{
    [Authorize]
    public partial class Downloads : ComponentBase
    {
        [Inject] public IUserProcessor userProcessor { get; set; }
        [Inject] public IDownloadProcessor downloadProcessor { get; set; }
        [Inject] public IMapper mapper { get; set; }
        [Inject] public AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] public NavigationManager navigationManager { get; set; }

        private List<Download> _downloads;
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _downloads = await Task.Run(async () => mapper.Map<List<Download>>(await downloadProcessor.GetDownloadsByAuthor(_context.User.GetUserId())));
        }
    }
}
