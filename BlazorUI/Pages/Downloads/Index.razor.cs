using AutoMapper;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Downloads
{
    public partial class Index : ComponentBase
    {
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }

        private List<Download> _downloads;

        protected override async Task OnInitializedAsync()
        {
            _downloads = await Task.Run(async () => mapper.Map<List<Download>>(await downloadProcessor.GetDownloads()));
        }
    }
}
