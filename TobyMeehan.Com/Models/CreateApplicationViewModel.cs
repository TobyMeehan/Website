using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class CreateApplicationViewModel
    {
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public ApplicationType Type { get; set; }

        public enum ApplicationType
        {
            WebServer,
            SinglePage,
            Native,
            Mobile
        }
    }
}
