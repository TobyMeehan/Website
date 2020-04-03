using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class ChangePasswordFormModel
    {
        [Required(ErrorMessage = "Please enter your current password.")]    
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter new password.")]
        [Display(Name = "New Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 - 100 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm new password.")]
        [Display(Name = "Confirm New Password")]
        [CompareProperty(nameof(NewPassword), ErrorMessage = "Passwords must match.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

    }
}
