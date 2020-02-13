using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public TokenController(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        /* POST https://api.authorization-server.com/token
            grant_type=authorization_code&
            code=AUTH_CODE_HERE&
            redirect_uri=REDIRECT_URI&
            client_id=CLIENT_ID&
            client_secret=CLIENT_SECRET*/
        public async Task<IActionResult> Post(AuthCodeTokenRequest request)
        {
            // TODO: GetConnectionByAuthCode
            ConnectionModel connection = new ConnectionModel();

            return Token(connection);
        }

        /* POST https://api.authorization-server.com/token
            grant_type=password&
            username=USERNAME&
            password=PASSWORD&
            client_id=CLIENT_ID */
        public async Task<IActionResult> Post(PasswordTokenRequest request)
        {
            // TODO: authorize user then get application by client id
            ConnectionModel connection = new ConnectionModel();

            return Token(connection);
        }

        /* POST https://api.authorization-server.com/token
            grant_type=client_credentials&
            client_id=CLIENT_ID&
            client_secret=CLIENT_SECRET */
        public async Task<IActionResult> Post(ClientCredentialTokenRequest request)
        {
            // TODO: get application by client id
            ConnectionModel connection = new ConnectionModel();

            return Token(connection);
        }

        public IActionResult Token(ConnectionModel connection)
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
 
 