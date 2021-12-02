using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class ClaimsPrincipleBuilder
    {
        private List<Claim> _claims = new List<Claim>();
        private string _authenticationScheme;

        public ClaimsPrincipleBuilder WithClaims(params Claim[] claims)
        {
            return WithClaims(claims.ToList());
        }

        public ClaimsPrincipleBuilder WithClaims(IEnumerable<Claim> claims)
        {
            _claims.AddRange(claims);

            return this;
        }

        public ClaimsPrincipleBuilder WithAuthenticationScheme(string scheme)
        {
            _authenticationScheme = scheme;

            return this;
        }

        public ClaimsPrincipal Build()
        {
            var identity = new ClaimsIdentity(_claims, _authenticationScheme);

            return new ClaimsPrincipal(identity);
        }
    }
}
