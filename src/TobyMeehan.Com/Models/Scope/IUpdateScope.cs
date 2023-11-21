namespace TobyMeehan.Com.Models.Scope;

public interface IUpdateScope
{
    Optional<string> DisplayName { get; }
    Optional<string> Description { get; }
}