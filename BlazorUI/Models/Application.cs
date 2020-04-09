using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class Application
    {
        public string Id { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public string Secret { get; set; }
        public string Role { get; set; }
    }
}
