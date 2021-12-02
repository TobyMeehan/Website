using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class RemoteCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private string GetReturnUrl(HttpContext context)
        {
            return $"?ReturnUrl={WebUtility.UrlEncode($"{context.Request.Host}{context.Request.Path}")}";
        }

        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
#if DEBUG
            context.HttpContext.Response.Redirect($"https://localhost:5001/login{GetReturnUrl(context.HttpContext)}");
#else
            context.HttpContext.Response.Redirect($"https://tobymeehan.com/login{GetReturnUrl(context.HttpContext)}");
#endif
            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
#if DEBUG
            context.HttpContext.Response.Redirect($"https://localhost:5001/login{GetReturnUrl(context.HttpContext)}");
#else
            context.HttpContext.Response.Redirect($"https://tobymeehan.com/login{GetReturnUrl(context.HttpContext)}");
#endif
            return Task.CompletedTask;
        }
    }
}
