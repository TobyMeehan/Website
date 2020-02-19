using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class ConnectionModel
    {
        public int ConnectionId { get; set; }
        public string AppId { get; set; }
        public ApplicationModel Application { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public string Token { get; set; }
        public string AuthorizationCode { get; set; }
    }
}
