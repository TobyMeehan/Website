using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ConnectionModel
    {
        public string ConnectionId { get; set; }
        public ClientModel Application { get; set; }
        public UserModel User { get; set; }
        public string Token { get; set; }

    }
}
