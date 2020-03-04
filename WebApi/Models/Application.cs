using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Application
    {
        [Display(Name = "App ID")]
        public string Id { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }
        [Display(Name = "Client Secret")]
        public string Secret { get; set; }
        public string Role { get; set; }

    }
}
