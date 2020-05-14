using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class AuthorizationCode : EntityBase
    {
        public string ConnectionId { get; set; }
        public Connection Connection { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public string CodeChallenge { get; set; }
    }
}
