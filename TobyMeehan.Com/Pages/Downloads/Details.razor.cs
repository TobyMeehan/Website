using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Details : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;

        protected override async Task OnInitializedAsync()
        {
            _download = await Task.Run(() => downloads.GetByIdAsync(Id));
        }

        private async Task VerifyForm_Submit()
        {
            await downloads.UpdateAsync(_download);

            // TODO: alerts
        }
    }
}
