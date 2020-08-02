using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.OAuth
{
    [Route("/oauth/[controller]")]
    [Authorize]
    public class AuthorizeController : Controller
    {
        private readonly IConnectionRepository _connections;
        private readonly IOAuthSessionRepository _sessions;
        private readonly IMapper _mapper;

        public AuthorizeController(IConnectionRepository connections, IOAuthSessionRepository sessions, IMapper mapper)
        {
            _connections = connections;
            _sessions = sessions;
            _mapper = mapper;
        }

        private bool ValidateApplication(ApplicationModel application, string clientId, string redirectUri)
        {
            return application.Id == clientId && new Uri(application.RedirectUri) == new Uri(redirectUri);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string client_id, string redirect_uri, string scope, string state, string code_challenge)
        {
            var connection = _mapper.Map<ConnectionModel>(await _connections.GetOrCreateAsync(User.Id(), client_id));

            if (connection == null)
            {
                return NotFound("Client ID could not be found.");
            }

            if (!ValidateApplication(connection.Application, client_id, redirect_uri))
            {
                return BadRequest("Client credentials were invalid.");
            }

            ViewBag.RedirectUri = redirect_uri;

            var scopes = scope?.Split(' ').Select(x => x.ToLower()) ?? new List<string>();

            if (!scopes.Any(x => Roles.Scopes.All.Contains(x)))
            {
                return Error(redirect_uri, nameof(scope), "No scopes were provided.");
            }

            return View(new AuthorizeViewModel { Connection = connection, Scopes = scope.Split(' ').Select(x => x.ToLower()) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] string ConnectionId, string client_id, string redirect_uri, string scope, string state, string code_challenge)
        {
            var session = await _sessions.AddAsync(ConnectionId, redirect_uri, scope ?? "", code_challenge);

            string returnCode = WebUtility.UrlEncode(session.AuthorizationCode);
            string returnState = WebUtility.UrlEncode(state);

            return Redirect($"{redirect_uri}?code={returnCode}&state={returnState}");
        }

        [Route("cancel")]
        public IActionResult Cancel(string redirect_uri)
        {
            return Error(redirect_uri, "access_denied", "User denied the request.");
        }

        private IActionResult Error(string redirectUri, string error, string message)
        {
            return Redirect($"{redirectUri}?error={WebUtility.UrlEncode(error)}&error_message={WebUtility.UrlEncode(message)}");
        }
    }
}