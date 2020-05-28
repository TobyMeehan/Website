using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("authorizationcodes")]
    public class AuthorizationCode : EntityBase
    {
        public string ConnectionId { get; set; }
        public Connection Connection { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public string CodeChallenge { get; set; }
    }
}
