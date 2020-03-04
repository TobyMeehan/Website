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

        /* POST https://api.authorization-server.com/token
            grant_type=authorization_code&
            code=AUTH_CODE_HERE&
            redirect_uri=REDIRECT_URI&
            client_id=CLIENT_ID&
            client_secret=CLIENT_SECRET*/
        public async Task<IActionResult> Post(AuthCodeTokenRequest request)
        {
            if (await _connectionProcessor.ValidateAuthCode(request.code))
            {
                Connection connection = _mapper.Map<Connection>(await _connectionProcessor.GetConnectionByAuthCode(request.code));

                if (connection.Application.Id == request.client_id && connection.Application.Secret == request.client_secret)
                {
                    return Token(connection);
                }
            }

            return Forbid();
        }

        /* POST https://api.authorization-server.com/token
            grant_type=password&
            username=USERNAME&
            password=PASSWORD&
            client_id=CLIENT_ID */
        public async Task<IActionResult> Post(PasswordTokenRequest request)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(request.client_id));

            if (app != null)
            {
                if (app.Role == ApplicationRoles.FirstParty)
                {
                    if (await _userProcessor.UserExists(request.username))
                    {
                        if (await _userProcessor.Authenticate(request.username, request.password))
                        {
                            Connection connection = new Connection
                            {
                                User = _mapper.Map<User>(await _userProcessor.GetUserByUsername(request.username)),
                                Application = app
                            };

                            connection = _mapper.Map<Connection>(await _connectionProcessor.CreateConnection(_mapper.Map<DataAccessLibrary.Models.Connection>(connection)));

                            return Token(connection);
                        }
                    }
                }
            }

            return Forbid();
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

        public JsonResult Token(Connection connection)
        {
            DateTime expiry = DateTime.UtcNow.AddDays(7);

            JsonWebToken token = new JsonWebToken
            {

                access_token = _tokenProvider.CreateToken(connection, expiry),
                expires_in = 60 * 60 * 24 * 7

            };

            return new JsonResult(token);
        }
    }
}
 
 