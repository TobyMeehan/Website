using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Extensions
{
    public static class RenderFragmentExtensions
    {
        // https://github.com/jsakamoto/Toolbelt.Blazor.HeadElement/blob/master/Components/Title.razor
        // TODO: this is unsupported. If something goes wrong this is the issue
        public static string AsString(this RenderFragment renderFragment)
        {
            var renderTreeBuilder = new RenderTreeBuilder();
            renderFragment.Invoke(renderTreeBuilder);

            var frames = renderTreeBuilder.GetFrames();
            var content = new StringBuilder();

            foreach (var frame in frames.Array)
            {
                switch (frame.FrameType)
                {
                    case RenderTreeFrameType.Text:
                        content.Append(frame.TextContent);
                        break;
                    case RenderTreeFrameType.Markup:
                        content.Append(frame.MarkupContent);
                        break;
                    default:
                        break;
                }
            }

            return content.ToString();
        }
    }
}
