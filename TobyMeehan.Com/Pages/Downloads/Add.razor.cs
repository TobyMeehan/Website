using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Add : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [CascadingParameter] public Task<AuthenticationState> authenticationStateTask { get; set; }

        private DownloadViewModel _form = new DownloadViewModel();
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateTask;
        }

        private async Task Form_Submit()
        {
            Download download = await downloads.AddAsync(_form.Title, _form.ShortDescription, _form.LongDescription, _context.User.Id());

            navigation.NavigateTo($"/downloads/{download.Id}/files");
        }
    }
}
