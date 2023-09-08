using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for applications.
/// </summary>
public interface IApplicationRepository : IRepository<ApplicationData>
{
    // SELECT

    /// <summary>
    /// Selects all applications with the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ApplicationData>> SelectByAuthorAsync(string userId, CancellationToken cancellationToken);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect data to the application.
    /// </summary>
    /// <param name="redirect"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddRedirectAsync(RedirectData redirect, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the specified redirect from the application.
    /// </summary>
    /// <param name="redirectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveRedirectAsync(string redirectId, CancellationToken cancellationToken);
}