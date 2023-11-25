namespace TobyMeehan.Com.Data.Entities;

public class User : IUser
{
    public required Id<IUser> Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Username { get; init; }
    public required double Balance { get; init; }
    public required string? Description { get; init; }
    public IAvatar? Avatar { get; set; }
    public required IEntityCollection<IUserRole> Roles { get; init; }
}