using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.OAuth
{
    [Route("/oauth/[controller]")]
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
            return application.Id == clientId && application.RedirectUri == redirectUri;
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

            return View(connection);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] string connectionId, string client_id, string redirect_uri, string scope, string state, string code_challenge)
        {
            var session = await _sessions.AddAsync(connectionId, code_challenge);

            string returnCode = WebUtility.UrlEncode(session.AuthorizationCode);
            string returnState = WebUtility.UrlEncode(state);

            return Redirect($"{redirect_uri}?code={returnCode}&state={returnState}");
        }
    }
}