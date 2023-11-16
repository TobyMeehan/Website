namespace TobyMeehan.Com;

public interface IAuthorization : IEntity<IAuthorization>
{
    Id<IApplication> ApplicationId { get; }
    Id<IUser> UserId { get; }
    
    string? Status { get; }
    string? Type { get; }
    IEntityCollection<IScope> Scopes { get; }
    DateTime CreatedAt { get; }
}