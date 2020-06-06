using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Index : ComponentBase
    {
        [Inject] private IRepository<Download> downloads { get; set; }

        private IList<Download> _downloads;

        protected override async Task OnInitializedAsync()
        {
            _downloads = await Task.Run(downloads.GetAsync);
        }
    }
}
