using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.Api.Models.OAuth;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Api.Controllers.OAuth
{
    [Route("oauth/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOAuthSessionRepository _sessions;
        private readonly IApplicationRepository _applications;
        private readonly IMapper _mapper;

        public TokenController(IOAuthSessionRepository sessions, IApplicationRepository applications, IMapper mapper)
        {
            _sessions = sessions;
            _applications = applications;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] AuthCodeTokenRequest request)
        {
            var session = await _sessions.GetByAuthCodeAsync(request.Code);

            if (session == null)
            {
                return BadRequest(new ErrorResponse("Invalid authorization code."));
            }

            if (!await _applications.ValidateAsync(request.ClientId, request.ClientSecret, request.RedirectUri, session.CodeChallenge == new SHA256Managed().ComputeCodeChallenge(request.CodeVerifier ?? "")))
            {
                return BadRequest(new ErrorResponse("Invalid application credentials."));
            }

            if (session.RedirectUri != request.RedirectUri)
            {
                return BadRequest(new ErrorResponse("Inconsistent redirect URI."));
            }

            var token = await _sessions.GenerateToken(session);

            return Ok(_mapper.Map<JsonWebTokenResponse>(token));
        }
    }
}