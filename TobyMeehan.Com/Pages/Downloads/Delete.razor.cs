using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Delete : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;

        protected override async Task OnInitializedAsync()
        {
            _download = await Task.Run(() => downloads.GetByIdAsync(Id));

            editDownloadState.Id = _download.Id;
            editDownloadState.Title = _download.Title;
        }

        private async Task ConfirmDelete()
        {
            await downloads.DeleteAsync(_download.Id);

            navigation.NavigateTo("/downloads");
        }
    }
}
