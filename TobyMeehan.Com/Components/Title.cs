using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Components
{
    public class Title : ComponentBase
    {
        [Inject] public JavaScript js { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            string content = ChildContent.AsString();
            string title = (!string.IsNullOrWhiteSpace(content)) ? $"{content} - " : "";

            await js.SetTitle($"{title}TobyMeehan");
        }
    }
}
