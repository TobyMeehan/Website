using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Components
{
    public partial class AwaitTask<T> : ComponentBase
    {
        [Parameter] public Task<T> Task { get; set; }
        [Parameter] public RenderFragment<T> ChildContent { get; set; }

        private T _item;

        protected override async Task OnInitializedAsync()
        {
            _item = await Task;
        }
    }
}
