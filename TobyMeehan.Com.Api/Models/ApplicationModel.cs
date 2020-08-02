using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class ApplicationModel : EntityModel
    {
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public string Secret { get; set; }
        public string IconUrl { get; set; }
        public UserModel Author { get; set; }
    }
}
