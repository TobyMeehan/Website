namespace TobyMeehan.Com.Data.Entities;

public class Scope : IScope
{
    public required Id<IScope> Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required IEntityCollection<IUserRole> UserRoles { get; init; }
}