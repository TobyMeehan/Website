using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class UsernameFormModel
    {
        [Required]
        public string Username { get; set; }

    }
}
