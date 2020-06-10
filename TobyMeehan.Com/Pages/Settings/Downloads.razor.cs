using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Downloads : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private IList<Download> _downloads;

        protected async override Task OnInitializedAsync()
        {
            _downloads = await Task.Run(() => downloads.GetByAuthorAsync(CurrentUser.Id));
        }
    }
}
