using System.ComponentModel.DataAnnotations;

namespace TobyMeehan.Com.Data.Repositories.Models;

public class UserData
{
    [Required]
    public string Id { get; set; }
    
    [StringLength(40, MinimumLength = 1)]
    public string? Name { get; set; }
    [StringLength(40, MinimumLength = 1)]
    [Required]
    [RegularExpression("([a-zA-Z0-9_-]+)")]
    public string Handle { get; set; }
    [Required]
    public string HashedPassword { get; set; }
    [Required]
    public double Balance { get; set; }
    [StringLength(400, MinimumLength = 1)]
    public string? Description { get; set; }
}