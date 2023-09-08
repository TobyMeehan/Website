using System.ComponentModel.DataAnnotations;

namespace TobyMeehan.Com.Accounts.Models;

public class ApplicationFormModel
{
    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    [Url]
    public string RedirectUri { get; set; }
}