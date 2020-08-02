using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("applications")]
    public class Application : EntityBase
    {
        public string UserId { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string RedirectUri { get; set; }
        public string Secret { get; set; }
        public string Role { get; set; }

        public bool Validate(string clientId, string secret, string redirectUri, bool ignoreSecret) => new List<bool>
        {

            Id == clientId,
            Secret == secret || ignoreSecret,
            RedirectUri == redirectUri

        }.TrueForAll(x => x);
    }
}
