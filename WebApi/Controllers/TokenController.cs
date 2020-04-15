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

namespace WebApi.Controllers
{
    [Route("api/token")]
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

            bool ignoreSecret = false;

            if (request.code_verifier != null)
            {
                if (await _connectionProcessor.ValidatePkce(new DataAccessLibrary.Models.Pkce
                {
                    ClientId = request.client_id,
                    CodeVerifier = request.code_verifier
                }))
                {
                    ignoreSecret = true;
                }
            }

            if (authCode?.IsValid ?? false)
            {
                if (await _applicationProcessor.ValidateApplication(request.client_id, request.client_secret, request.redirect_uri, ignoreSecret)
                    && request.client_id == authCode.Connection.Application.Id)
                {
                    return Token(authCode.Connection);
                }
            }

            return Unauthorized();
        }

        /* POST https://api.authorization-server.com/token
            grant_type=password&
            username=USERNAME&
            password=PASSWORD&
            client_id=CLIENT_ID */
        public async Task<ActionResult<JsonWebToken>> Post(PasswordTokenRequest request)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(request.client_id));

            if (app?.Role == ApplicationRoles.FirstParty)
            {
                if (await _userProcessor.Authenticate(request.username, request.password))
                {
                    Connection connection = new Connection
                    {
                        User = _mapper.Map<User>(await _userProcessor.GetUserByUsername(request.username)),
                        Application = app
                    };


                    return Token(connection);
                }
            }

            return Unauthorized();
        }

        // TODO: Consider whether client credentials is necessary
        /* POST https://api.authorization-server.com/token
            grant_type=client_credentials&
            client_id=CLIENT_ID&
            client_secret=CLIENT_SECRET */
        //public async Task<IActionResult> Post(ClientCredentialTokenRequest request)
        //{
        //    // TODO: get application by client id
        //    ConnectionModel connection = new ConnectionModel();

        //    return Token(connection);
        //}

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

