using AutoMapper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Edit : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private DownloadViewModel _form;
        private ServerSideValidator _serverSideValidator;

        protected override async Task OnInitializedAsync()
        {
            _download = await Task.Run(() => downloads.GetByIdAsync(Id));
            _form = await _download.AsAsync<DownloadViewModel>(mapper);

            editDownloadState.Id = _download.Id;
            editDownloadState.Title = _download.Title;
        }

        private async Task EditForm_Submit()
        {
            if (_form.Version < _download.Version)
            {
                _serverSideValidator.Error(nameof(_form.Version), "New version must be greater than or equal to current version.");
                return;
            }

            _download.Title = _form.Title;
            _download.ShortDescription = _form.ShortDescription;
            _download.LongDescription = _form.LongDescription;
            _download.Version = _form.Version;
            _download.Visibility = _form.Visibility;

            await downloads.UpdateAsync(Id, _download);
            editDownloadState.Title = _form.Title;
        }
    }
}
