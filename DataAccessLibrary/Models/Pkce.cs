using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Pkce
    {
        public string ClientId { get; set; }
        public string CodeChallenge { get; set; }
        public string CodeVerifier { get; set; }

    }
}
