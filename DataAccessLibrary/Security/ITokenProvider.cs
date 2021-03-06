﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TobyMeehan.Com.Data.Security
{
    public interface ITokenProvider
    {
        string CreateToken(List<Claim> claims, DateTime expiry);
        TokenValidationParameters GetValidationParameters();
    }
}