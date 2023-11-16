namespace TobyMeehan.Com;

public interface IScope : IEntity<IScope>
{
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }
}