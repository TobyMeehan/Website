using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Models.Api;

namespace TobyMeehan.Com.Api.Controllers
{
    public class OAuthControllerBase : ControllerBase
    {
        public string UserId => User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        public string AppId => User.Claims.First(x => x.Type == ClaimTypes.Actor).Value;

        protected IActionResult Forbid(ErrorResponse error)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, error);
        }
    }
}
