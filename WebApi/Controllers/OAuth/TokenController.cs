using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Jwt;
using WebApi.Models;

namespace WebApi.Controllers.OAuth
{
    [Route("oauth/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IMapper _mapper;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;

        public TokenController(ITokenProvider tokenProvider, IMapper mapper, IConnectionProcessor connectionProcessor, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor)
        {
            _tokenProvider = tokenProvider;
            _mapper = mapper;
            _connectionProcessor = connectionProcessor;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
        }

        [HttpPost]
        public async Task<ActionResult<JsonWebToken>> Post([FromBody] AuthCodeTokenRequest bodyRequest, [FromForm] AuthCodeTokenRequest formRequest)
        {
            AuthCodeTokenRequest request = bodyRequest ?? formRequest;

            AuthorizationCode authCode = _mapper.Map<AuthorizationCode>(await _connectionProcessor.GetAuthorizationCode(request.code));

            if (authCode == null)
            {
                return Unauthorized();
            }

            bool ignoreSecret = _connectionProcessor.CheckPkce(authCode.CodeChallenge, request.code_verifier);

            if (await _applicationProcessor.ValidateApplication(request.client_id, request.client_secret, request.redirect_uri, ignoreSecret)
                && request.client_id == authCode.Connection.Application.Id)
            {
                return Token(authCode.Connection);
            }

            return Unauthorized();
        }

        public async Task<ActionResult<JsonWebToken>> Post(PasswordTokenRequest request)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(request.client_id));

            if (app?.Role != ApplicationRoles.FirstParty)
            {
                return Forbid();
            }

            if (await _userProcessor.Authenticate(request.username, request.password))
            {
                Connection connection = new Connection
                {
                    User = _mapper.Map<User>(await _userProcessor.GetUserByUsername(request.username)),
                    Application = app
                };


                return Token(connection);
            }
            else
            {
                return Unauthorized();
            }
        }

        private ActionResult<JsonWebToken> Token(Connection connection)
        {
            DateTime expiry = DateTime.UtcNow.AddDays(7);

            JsonWebToken token = new JsonWebToken
            {

                access_token = _tokenProvider.CreateToken(connection, expiry),
                expires_in = 60 * 60 * 24 * 7

            };

            return token;
        }
    }
}

