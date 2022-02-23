using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TobyMeehan.Com.Data.Configuration;

namespace TobyMeehan.Com.Data.Security
{
    public class SymmetricTokenProvider : ITokenProvider
    {
        private readonly SymmetricTokenOptions _options;

        public SymmetricTokenProvider(IOptions<SymmetricTokenOptions> options)
        {
            _options = options.Value;
        }
        public string CreateToken(List<Claim> claims, DateTime expiry)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(_options.Issuer, _options.Audience, claims,
                expires: expiry.ToUniversalTime(), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            throw new NotImplementedException();
        }
    }
}