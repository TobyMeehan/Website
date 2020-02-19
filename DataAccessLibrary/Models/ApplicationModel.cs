using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class ApplicationModel
    {
        public string AppId { get; set; }
        public string UserId { get; set; }
        public UserModel Author { get; set; }
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public string Secret { get; set; }
        public string Role { get; set; }

    }
}
