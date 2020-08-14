using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace TobyMeehan.Com.Data.Security
{
    public class DefaultTokenProvider : ITokenProvider
    {
        public string CreateToken(List<Claim> claims, DateTime expiry)
        {
            throw new NotImplementedException();
        }

        public TokenValidationParameters GetValidationParameters()
        {
            throw new NotImplementedException();
        }
    }
}
