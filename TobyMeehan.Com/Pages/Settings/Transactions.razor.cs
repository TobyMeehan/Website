using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Transactions : ComponentBase
    {
        [Inject] private ITransactionRepository transactions { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter] public User CurrentUser { get; set; }

        private List<IGrouping<DateTime, Transaction>> _transactions;

        protected override async Task OnInitializedAsync()
        {
            var ts = await Task.Run(async () => await transactions.GetByUserAsync(CurrentUser.Id));

            _transactions = ts.OrderByDescending(x => x.Sent).GroupBy(x => x.Sent.Date).ToList();
        }
    }
}
