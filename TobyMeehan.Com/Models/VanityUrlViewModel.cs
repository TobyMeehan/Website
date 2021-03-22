using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class VanityUrlViewModel
    {
        [MaxLength(50, ErrorMessage = "Url should be no longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Url cannot contain special characters or spaces.")]
        public string VanityUrl { get; set; }
    }
}
