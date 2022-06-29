using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for application data.
/// </summary>
public interface IApplicationService
{
    // GET

    /// <summary>
    /// Gets all applications for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEntityCollection<IApplication>> GetByUserAsync(Id<IUser> user);

    /// <summary>
    /// Gets the application with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IApplication> GetByIdAsync(Id<IApplication> id);
    
    // CREATE

    /// <summary>
    /// Creates a new application with the specified builder.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    Task<IApplication> CreateAsync(CreateApplicationBuilder application);
    
    // UPDATE

    /// <summary>
    /// Updates the specified application with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="application"></param>
    /// <returns></returns>
    Task<IApplication> UpdateAsync(Id<IApplication> id, UpdateApplicationBuilder application);
    
    // DELETE

    /// <summary>
    /// Deletes the specified application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IApplication> id);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect URI to the specified application.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<IRedirect> AddRedirectAsync(Id<IApplication> id, Uri uri);

    /// <summary>
    /// Removes the specified redirect.
    /// </summary>
    /// <param name="redirect"></param>
    /// <returns></returns>
    Task RemoveRedirectAsync(Id<IRedirect> redirect);
}