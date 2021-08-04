using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Admin
{
    public partial class Index : ComponentBase
    {
        [Inject] public IUserRepository users { get; set; }

        private IEnumerable<User> _users;

        protected override async Task OnInitializedAsync()
        {
            _users = await Task.Run(users.GetAsync);
        }
    }
}
