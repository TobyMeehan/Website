namespace TobyMeehan.Com.Models.Scope;

public interface ICreateScope
{
    Optional<string> Alias { get; }
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }
}