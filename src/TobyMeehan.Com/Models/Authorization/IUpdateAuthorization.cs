namespace TobyMeehan.Com.Models.Authorization;

public interface IUpdateAuthorization
{
    Optional<string?> Status { get; }
    Optional<string?> Type { get; }
    Optional<IEnumerable<string>> Scopes { get; }
    Optional<DateTime> CreatedAt { get; }
}