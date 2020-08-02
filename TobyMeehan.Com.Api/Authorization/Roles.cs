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
                Identify,
                Transactions,
                Downloads,
                Connections,
                Applications
            };

            public const string Identify = "identify";
            public const string Transactions = "transactions";
            public const string Downloads = "downloads";
            public const string Connections = "connections";
            public const string Applications = "applications";
        }
    }
}
