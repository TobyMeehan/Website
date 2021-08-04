using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.AspNetCore.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Library : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private AuthenticationState _context;

        private IEnumerable<Download> _yourDownloads;
        private IEnumerable<Download> _unlisted;

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;

            _yourDownloads = await Task.Run(() => downloads.GetByAuthorAsync(_context.User.Id()));
            _unlisted = _yourDownloads.Where(d => d.Visibility == DownloadVisibility.Unlisted);
            _yourDownloads = _yourDownloads.Except(_unlisted);
        }
    }
}
