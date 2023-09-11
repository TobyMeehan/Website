namespace TobyMeehan.Com.Services;

public interface IConnectionService
{
    Task<IEntityCollection<IConnection>> GetByUserAsync(Id<IUser> user, CancellationToken cancellationToken = default);
    
    Task<IConnection> GetByIdAsync(Id<IConnection> id, CancellationToken cancellationToken = default);

    Task<IConnection> GetOrCreateAsync(Id<IUser> user, Id<IApplication> application, bool autoAuthorize,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Id<IConnection> id, CancellationToken cancellationToken = default);
}