using AutoMapper;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Developer
{
    public partial class Details : ComponentBase
    {
        [Inject] private IApplicationRepository applications { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private JavaScript js { get; set; }
        [Inject] public NavigationManager navigation { get; set; }

        [Parameter] public string Id { get; set; }

        private ServerSideValidator _serverSideValidator;

        private Application _application;
        private ApplicationViewModel _model = new ApplicationViewModel();

        protected override async Task OnInitializedAsync()
        {
            _application = await Task.Run(async () => await applications.GetByIdAsync(Id));
            _model = mapper.Map<ApplicationViewModel>(_application);
        }

        private async Task InformationForm_Submit()
        {
            await applications.UpdateAsync(mapper.Map<Application>(_model));
        }

        private async Task Delete_Click()
        {
            await applications.DeleteAsync(Id);
            navigation.NavigateTo("/developer");
        }

        private async Task Icon_Change(IEnumerable<IFileReference> files)
        {
            var file = files.FirstOrDefault();
            var info = await file.ReadFileInfoAsync();

            string url = await applications.AddIconAsync(Id, info.Name, info.Type, await file.OpenReadAsync());

            _application.IconUrl = url;
        }

        private async Task RemoveIcon_Click()
        {
            await applications.RemoveIconAsync(Id);
            _application.IconUrl = null;
        }
    }
}
