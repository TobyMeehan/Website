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
    /// <param name="id">The requested ID of the application, in string form.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The <see cref="IApplication"/> with the specified ID if found, null otherwise.</returns>
    Task<IApplication?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to find an application with the specified ID and secret.
    /// </summary>
    /// <param name="id">The ID of the application, in string form.</param>
    /// <param name="secret">The secret to be compared with the stored hash.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns></returns>
    Task<IApplication?> FindByCredentialsAsync(string id, Password secret,
        CancellationToken cancellationToken = default);
    
    // GET

    /// <summary>
    /// Gets all applications belonging to the specified user.
    /// </summary>
    /// <param name="user">The ID of the author.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IEntityCollection{T}"/> containing all applications belonging to the user, collection will be empty if none were found.</returns>
    Task<IEntityCollection<IApplication>> GetByAuthorAsync(Id<IUser> user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the application with the specified ID, throws an <see cref="EntityNotFoundException{T}"/> if the application does not exist.
    /// </summary>
    /// <param name="id">The requested ID of the application.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IApplication"/> representing the application with the specified ID.</returns>
    Task<IApplication> GetByIdAsync(Id<IApplication> id, CancellationToken cancellationToken = default);
    
    // CREATE

    /// <summary>
    /// Creates a new application with the specified builder.
    /// </summary>
    /// <param name="application"><see cref="CreateApplicationBuilder"/> with the required options.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IApplication"/> representing the created application.</returns>
    Task<IApplication> CreateAsync(CreateApplicationBuilder application, CancellationToken cancellationToken = default);
    
    // UPDATE

    /// <summary>
    /// Updates the specified application with the specified builder.
    /// </summary>
    /// <param name="id">ID of the application to be updated.</param>
    /// <param name="application"><see cref="UpdateApplicationBuilder"/> with the required properties to be changed.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IApplication"/> representing the resulting updated application.</returns>
    Task<IApplication> UpdateAsync(Id<IApplication> id, UpdateApplicationBuilder application, CancellationToken cancellationToken = default);
    
    // DELETE

    /// <summary>
    /// Deletes the specified application.
    /// </summary>
    /// <param name="id">ID of the application to be deleted.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns></returns>
    Task DeleteAsync(Id<IApplication> id, CancellationToken cancellationToken = default);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect URI to the specified application.
    /// </summary>
    /// <param name="id">ID of the application to add the specified redirect to.</param>
    /// <param name="uri"><see cref="Uri"/> object representing the redirect URI to be added.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns><see cref="IRedirect"/> representing the added redirect.</returns>
    Task<IRedirect> AddRedirectAsync(Id<IApplication> id, Uri uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified redirect.
    /// </summary>
    /// <param name="redirect">ID of the redirect to be removed.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns></returns>
    Task RemoveRedirectAsync(Id<IRedirect> redirect, CancellationToken cancellationToken = default);
}