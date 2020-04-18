using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class AuthorizationCode
    {
        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public Connection Connection { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public string CodeChallenge { get; set; }

    }
}
