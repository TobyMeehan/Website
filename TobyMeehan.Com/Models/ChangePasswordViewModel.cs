using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your current password.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter a new password.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password should be between 8 - 50 characters.")]
        public string NewPassword { get; set; }

        [CompareProperty(nameof(NewPassword), ErrorMessage = "Passwords must match.")]
        public string ConfirmNewPassword { get; set; }

    }
}
