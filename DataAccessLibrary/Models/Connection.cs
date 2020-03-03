using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public Application Application { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public string AuthorizationCode { get; set; }
    }
}
