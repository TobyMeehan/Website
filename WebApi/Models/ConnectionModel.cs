using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ConnectionModel
    {
        public ApplicationModel Application { get; set; }
        public UserModel User { get; set; }
        public string Token { get; set; }
        public string AuthorizationCode { get; set; }
    }
}
