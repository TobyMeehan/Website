using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class JsonWebToken
    {
        public string access_token { get; set; }

        public string token_type { get; set; } = "bearer";

        public int expires_in { get; set; }

        public string refresh_token { get; set; }

    }
}
