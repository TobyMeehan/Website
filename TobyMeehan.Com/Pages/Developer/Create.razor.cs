using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Developer
{
    public partial class Create : ComponentBase
    {
        [Inject] private IApplicationRepository applications { get; set; }
        [Inject] public NavigationManager navigation { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private CreateApplicationViewModel _model = new CreateApplicationViewModel();
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;
        }

        private async Task Form_Submit()
        {
            var application = await applications.AddAsync(_context.User.Id(), _model.Name, _model.RedirectUri, _model.Type == CreateApplicationViewModel.ApplicationType.WebServer);

            navigation.NavigateTo($"/developer/applications/{application.Id}");
        }
    }
}
