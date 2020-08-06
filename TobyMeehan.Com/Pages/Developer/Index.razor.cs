using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Pages.Developer
{
    public partial class Index : ComponentBase
    {
        [Inject] private IApplicationRepository applications { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private IList<Application> _applications = new List<Application>();

        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;
            _applications = await Task.Run(async () => await applications.GetByUserAsync(_context.User.Id()));
        }
    }
}
