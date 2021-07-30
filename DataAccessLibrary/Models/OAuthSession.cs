using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class OAuthSession : EntityBase
    {
        public string ConnectionId { get; set; }
        public Connection Connection { get; set; }
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }

        public string Scope { get; set; }
        public IEnumerable<string> Scopes => Scope.Split(' ');

        public string CodeChallenge { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
