using SqlKata;

namespace TobyMeehan.Com.Data.Models;

public class ScopeDto
{
    public required string Id { get; set; }
    public required string Alias { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }

    [Ignore] public IReadOnlyList<RoleDto> UserRoles { get; set; } = new List<RoleDto>();
}