using Microsoft.IdentityModel.Tokens;
using System;
using WebApi.Models;

namespace WebApi.Jwt
{
    public interface ITokenProvider
    {
        string CreateToken(Connection connection, DateTime expiry);
        TokenValidationParameters GetValidationParameters();
    }
}