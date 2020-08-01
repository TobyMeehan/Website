using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("oauthsessions")]
    public class OAuthSession : EntityBase
    {
        public string ConnectionId { get; set; }
        public Connection Connection { get; set; }
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
        public string CodeChallenge { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
