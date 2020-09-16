using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Authorization
{
    public static class Scopes
    {
        public const string ClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        public const string Identify = "identify";
        public const string Transactions = "transactions";
        public const string Downloads = "downloads";
        public const string Connections = "connections";
        public const string Applications = "applications";
    }
}
