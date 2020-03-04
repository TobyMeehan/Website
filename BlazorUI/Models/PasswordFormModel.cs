using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class PasswordFormModel
    {
        [Required(ErrorMessage = "Enter your password.")]
        public string Password { get; set; }

    }
}
