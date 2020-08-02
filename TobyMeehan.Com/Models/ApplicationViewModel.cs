using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class ApplicationViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Application needs a name.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 128 characters.")]
        public string Name { get; set; }

        [MaxLength(400, ErrorMessage = "Maximum 400 characters.")]
        public string Description { get; set; }
        public string IconUrl { get; set; }

        [Required(ErrorMessage = "Redirect URI is required.")]
        public string RedirectUri { get; set; }
        
    }
}
