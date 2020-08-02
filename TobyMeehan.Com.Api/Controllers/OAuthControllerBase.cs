using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Models.Api;

namespace TobyMeehan.Com.Api.Controllers
{
    public class OAuthControllerBase : ControllerBase
    {
        public string UserId => User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        public bool HasScope(string scope) => User.IsInRole($"Scope:{scope}");

        protected IActionResult Forbid(ErrorResponse error)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, error);
        }
    }
}
