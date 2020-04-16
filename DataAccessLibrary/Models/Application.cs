using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Application
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public string Secret { get; set; }
        public string Role { get; set; }
        public Scoreboard Scoreboard { get; set; }

    }
}
