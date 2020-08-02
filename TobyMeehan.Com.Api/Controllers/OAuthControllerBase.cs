using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Controllers
{
    public class OAuthControllerBase : ControllerBase
    {
        public string UserId => User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        public bool HasScope(string scope) => User.IsInRole($"Scope:{scope}");
    }
}
