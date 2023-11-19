namespace TobyMeehan.Com.Models.Authorization;

public interface ICreateAuthorization
{
    Optional<Id<IAuthorization>> Id { get; }
    Id<IApplication> Application { get; }
    Id<IUser> User { get; }
    string? Status { get; }
    string? Type { get; }
    IEnumerable<string> Scopes { get; }
    DateTime CreatedAt { get; }
}