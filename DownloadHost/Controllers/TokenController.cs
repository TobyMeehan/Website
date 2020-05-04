using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLibrary.Security;
using DownloadHost.Data;
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
        private readonly IFileManager _fileManager;

        public TokenController(IConfiguration configuration, ITokenProvider tokenProvider, IFileManager fileManager)
        {
            _configuration = configuration;
            _tokenProvider = tokenProvider;
            _fileManager = fileManager;
        }

        [HttpPost]
        public IActionResult Post(TokenRequest request)
        {
            string secret = _configuration.GetSection("UploadSecret").Value;

            if (request.Secret != secret)
            {
                return Forbid();
            }

            string randomFileName = _fileManager.GenerateRandomFilename(request.DownloadId);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, randomFileName),
                new Claim(ClaimTypes.NameIdentifier, request.DownloadId),
                new Claim("Partitions", request.Partitions.ToString())
            };

            DateTime expiry = DateTime.Now.AddHours(1);

            string token = _tokenProvider.CreateToken(claims, expiry);

            return Ok(new
            {
                token,
                randomName = randomFileName
            });
        }

        private string GetPath(string filename, string downloadId)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), downloadId, filename);
        }
    }
}