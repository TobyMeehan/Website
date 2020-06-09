using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Controllers
{
    [Authorize]
    public class OAuthController : Controller
    {
        private readonly IApplicationRepository _applications;
        private readonly IConnectionRepository _connections;
        private readonly IUserRepository _users;

        public OAuthController(IApplicationRepository applications, IConnectionRepository connections, IUserRepository users)
        {
            _applications = applications;
            _connections = connections;
            _users = users;
        }

        [HttpGet("/oauth/authorize")]
        public async Task<IActionResult> Authorize(string client_id, string redirect_uri, string state, string code_challenge)
        {
            Application application = await _applications.GetByIdAsync(client_id);
            User user = await _users.GetByIdAsync(User.Id());

            if (application == null)
            {
                return RedirectToAction(nameof(Error), new { code = ErrorCode.NotFound });
            }

            if (!application.Validate(client_id, null, redirect_uri, true))
            {
                return RedirectToAction(nameof(Error), new { code = ErrorCode.InvalidCredentials });
            }

            if ((await _connections.GetByUserAndApplicationAsync(User.Id(), application.Id)) == null)
            {
                return View(new Tuple<Application, User>(application, user));
            }
            else
            {
                return await AuthorizationCode(application.Id, application.RedirectUri, state, code_challenge);
            }
        }

        [HttpPost("/oauth/authorize")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ConfirmAuthorize(string client_id, string redirect_uri, string state, string code_challenge)
        {
            return AuthorizationCode(client_id, redirect_uri, state, code_challenge);
        }

        private async Task<IActionResult> AuthorizationCode(string clientId, string redirectUri, string state, string codeChallenge)
        {
            var code = await _connections.AddAsync(User.Id(), clientId, codeChallenge);

            string returnCode = WebUtility.UrlEncode(code.Code);
            string returnState = WebUtility.UrlEncode(state);

            return Redirect($"{redirectUri}?code={returnCode}&state={returnState}");
        }

        public enum ErrorCode
        {
            NotFound = 1,
            InvalidCredentials = 2
        }

        [HttpGet("/oauth/error/{code}")]
        public IActionResult Error(ErrorCode code)
        {
            return View(new OAuthErrorViewModel
            {
                Message = code switch
                {
                    ErrorCode.NotFound => "Could not find an application with the provided client ID.",
                    ErrorCode.InvalidCredentials => "One or more of the provided credentials for the application were invalid, for example the redirect URL may not have matched the one which was registered."
                }
            });
        }
    }
}