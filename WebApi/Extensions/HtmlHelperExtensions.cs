using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string ActivePage(this IHtmlHelper html, string controllers, string actions, string cssClass = "active")
        {
            string currentAction = html.ViewContext.RouteData.Values["action"] as string;
            string currentController = html.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(",");
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(",");

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : string.Empty;
        }
    }
}
