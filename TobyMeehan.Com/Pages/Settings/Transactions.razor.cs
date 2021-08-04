using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private List<IEnumerable<Transaction>> _transactions = new List<IEnumerable<Transaction>>();

        protected override async Task OnInitializedAsync()
        {
            await LoadNextPage();
        }

        private async Task LoadNextPage()
        {
            _transactions.Add(null);

            var page = await Task.Run(() => transactions.GetByUserAsync(CurrentUser.Id, _transactions.Count));

            _transactions.Remove(_transactions.Last());

            if (page.Any())
            {
                _transactions.Add(page);
            }
        }

        private string Amount(Transaction transaction)
        {
            StringBuilder sb = new StringBuilder($"£{Math.Abs(transaction.Amount)}");

            if (transaction.Amount < 0)
            {
                sb.Insert(0, "-");
            }

            return sb.ToString();
        }
    }
}
