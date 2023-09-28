using TobyMeehan.Com.Exceptions;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for connection data.
/// </summary>
public interface IConnectionService
{
    /// <summary>
    /// Attempts to find a connection with the specified ID.
    /// </summary>
    /// <param name="id">The requested ID of the connection, in string form.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The <see cref="IConnection"/> with the specified ID if found, null otherwise.</returns>
    Task<IConnection?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the connection with the specified ID, throws <see cref="EntityNotFoundException{T}"/> if the connection does not exist.
    /// </summary>
    /// <param name="id">The requested ID of the connection.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IConnection"/> representing the connection with the specified ID.</returns>
    Task<IConnection> GetByIdAsync(Id<IConnection> id, CancellationToken cancellationToken = default);
    
    Task<IEntityCollection<IConnection>> GetByUserAsync(Id<IUser> user, CancellationToken cancellationToken = default);
    
    Task<IConnection> GetOrCreateAsync(Id<IUser> user, Id<IApplication> application, bool autoAuthorize,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Id<IConnection> id, CancellationToken cancellationToken = default);
}