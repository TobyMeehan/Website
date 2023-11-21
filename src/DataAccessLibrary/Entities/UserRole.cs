namespace TobyMeehan.Com.Data.Entities;

public class UserRole : IUserRole
{
    public required Id<IUserRole> Id { get; init; }
    public required string Name { get; init; }
}