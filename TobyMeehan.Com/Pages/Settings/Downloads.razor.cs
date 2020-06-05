using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Downloads : ComponentBase
    {
        [Inject] private IRepository<Download> downloads { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private Task<IEnumerable<Download>> DownloadTask()
        {
            return downloads.GetByUser(CurrentUser.Id);
        }
    }
}
