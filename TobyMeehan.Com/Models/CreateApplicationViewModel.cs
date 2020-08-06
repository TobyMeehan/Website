using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class CreateApplicationViewModel
    {
        [Required(ErrorMessage = "Application needs a name.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 128 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Redirect URI is required.")]
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
