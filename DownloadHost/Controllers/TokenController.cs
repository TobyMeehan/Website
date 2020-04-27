using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLibrary.Security;
using DownloadHost.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DownloadHost.Controllers
{
    [Route("/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenProvider _tokenProvider;

        public TokenController(IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _configuration = configuration;
            _tokenProvider = tokenProvider;
        }

        [HttpPost]
        public IActionResult Post(TokenRequest request)
        {
            string secret = _configuration.GetSection("UploadSecret").Value;

            if (request.Secret != secret)
            {
                return Forbid();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Filename),
                new Claim(ClaimTypes.NameIdentifier, request.DownloadId)
            };

            DateTime expiry = DateTime.Now.AddHours(1);

            string token = _tokenProvider.CreateToken(claims, expiry);

            return Ok(new
            {
                token
            });
        }
    }
}