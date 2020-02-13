using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ClientCredentialTokenRequest
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

    }
}
