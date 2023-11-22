namespace TobyMeehan.Com;

public interface IScope : IEntity<IScope>
{
    string Alias { get; }
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }
    IEntityCollection<IUserRole> UserRoles { get; }
}