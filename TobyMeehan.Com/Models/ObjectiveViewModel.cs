using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class ObjectiveViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
