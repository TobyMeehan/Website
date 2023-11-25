namespace TobyMeehan.Com.Data.Domain.UserRoles.Models;

public class UserRole : IUserRole
{
    public required Id<IUserRole> Id { get; init; }
    public required string Name { get; init; }
}