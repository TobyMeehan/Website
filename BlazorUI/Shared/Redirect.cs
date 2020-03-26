using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Shared
{
    public class Redirect : ComponentBase
    {
        [Inject] public NavigationManager navigationManager { get; set; }
        [Parameter] public string Path { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            navigationManager.NavigateTo(Path, true);
        }
    }
}
