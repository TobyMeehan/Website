using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Authorization
{
    public static class Roles
    {
        public static class User
        {
            public const string Admin = nameof(Admin);
            public const string Verified = nameof(Verified);
        }

        public static class Scopes
        {
            public static IEnumerable<string> All => new List<string>
            {
                "identify",
                "transactions",
                "downloads",
                "connections",
                "applications"
            };

            public const string Identify = "Scope:identify";
            public const string Transactions = "Scope:transactions";
            public const string Downloads = "Scope:downloads";
            public const string Connections = "Scope:connections";
            public const string Applications = "Scope:applications";
        }
    }
}
