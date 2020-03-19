using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Shared
{
    public class Title : ComponentBase
    {
        [Inject] protected IJSRuntime jsRuntime { get; set; }

        [Parameter] public string Value { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Value = (!string.IsNullOrWhiteSpace(Value)) ? $"{Value} - " : "";
                await SetTitle($"{Value}TobyMeehan");
            }
        }

        private async Task SetTitle(string title)
        {
            await jsRuntime.InvokeVoidAsync("setTitle", title);
        }
    }
}
