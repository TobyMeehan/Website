using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Models.Api;

namespace TobyMeehan.Com.Api.Authorization
{
    public class ScopeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _scopes;

        public ScopeAttribute(params string[] scopes)
        {
            _scopes = scopes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_scopes.All(scope => context.HttpContext.User.IsInRole($"Scope:{scope}")))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                context.Result = new ObjectResult(new ErrorResponse($"Scopes {string.Join(", ", _scopes)} are required."));
            }
        }
    }
}
