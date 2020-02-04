using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ApplicationFormModel
    {
        [Required(ErrorMessage = "Enter a name for your application.")]
        [Display(Name = "Application Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a redirect URI for your application.")]
        [Display(Name = "Redirect Uri")]
        public string RedirectUri { get; set; }

        [Required(ErrorMessage = "Select an application type.")]
        [Display(Name = "Application Type")]
        public ApplicationType Type { get; set; }

        public enum ApplicationType
        {
            [Display(Name = "Web Server")]
            WebServer,
            [Display(Name = "Single Page")]
            SinglePage,
            [Display(Name = "Native")]
            Native,
            [Display(Name = "Mobile App")]
            Mobile
        }
    }
}
