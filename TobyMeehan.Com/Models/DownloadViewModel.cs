using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class DownloadViewModel
    {
        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a short description.")]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        [Required]
        public Version Version { get; set; } = new Version(1, 0, 0);
    }
}
