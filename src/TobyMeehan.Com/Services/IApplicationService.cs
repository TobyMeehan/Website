using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Exceptions;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for application data.
/// </summary>
public interface IApplicationService
{
    // FIND
    
    /// <summary>
    /// Attempts to find an application with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IApplication?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<IApplication?> FindByCredentialsAsync(string id, Password secret,
        CancellationToken cancellationToken = default);
    
    // GET

    /// <summary>
    /// Gets all applications for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEntityCollection<IApplication>> GetByAuthorAsync(Id<IUser> user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the application with the specified ID. Throws an <see cref="EntityNotFoundException{T}"/> if the application does not exist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IApplication> GetByIdAsync(Id<IApplication> id, CancellationToken cancellationToken = default);
    
    // CREATE

    /// <summary>
    /// Creates a new application with the specified builder.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    Task<IApplication> CreateAsync(CreateApplicationBuilder application, CancellationToken cancellationToken = default);
    
    // UPDATE

    /// <summary>
    /// Updates the specified application with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="application"></param>
    /// <returns></returns>
    Task<IApplication> UpdateAsync(Id<IApplication> id, UpdateApplicationBuilder application, CancellationToken cancellationToken = default);
    
    // DELETE

    /// <summary>
    /// Deletes the specified application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IApplication> id, CancellationToken cancellationToken = default);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect URI to the specified application.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<IRedirect> AddRedirectAsync(Id<IApplication> id, Uri uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified redirect.
    /// </summary>
    /// <param name="redirect"></param>
    /// <returns></returns>
    Task RemoveRedirectAsync(Id<IRedirect> redirect, CancellationToken cancellationToken = default);
}