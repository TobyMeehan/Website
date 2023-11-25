using SqlKata;
using TobyMeehan.Com.Data.Domain.Avatars.Models;
using TobyMeehan.Com.Data.Domain.UserRoles.Models;

namespace TobyMeehan.Com.Data.Domain.Users.Models;

public class UserDto
{
    public required string Id { get; set; }
    public string? AvatarId { get; set; }
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required string HashedPassword { get; set; }
    public required double Balance { get; set; }
    public string? Description { get; set; }

    [Ignore]
    public AvatarDto? Avatar { get; set; }
    
    [Ignore]
    public IReadOnlyList<RoleDto> Roles { get; set; } = new List<RoleDto>();
}