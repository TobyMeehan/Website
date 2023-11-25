namespace TobyMeehan.Com.Data.Domain.Authorizations.Models;

public class Authorization : IAuthorization
{
    public required Id<IAuthorization> Id { get; init; }
    public required Id<IApplication> ApplicationId { get; init; }
    public required Id<IUser> UserId { get; init; }
    public required string? Status { get; init; }
    public required string? Type { get; init; }
    public required IEntityCollection<IScope> Scopes { get; init; }
    public required DateTime CreatedAt { get; init; }
}