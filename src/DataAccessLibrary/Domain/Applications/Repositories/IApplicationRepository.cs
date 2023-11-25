using TobyMeehan.Com.Data.Domain.Applications.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Applications.Repositories;

/// <summary>
/// Database repository for applications.
/// </summary>
public interface IApplicationRepository
{
    // SELECT

    IAsyncEnumerable<ApplicationDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    
    /// <summary>
    /// Selects all applications with the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="limit"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<ApplicationDto> SelectByAuthorAsync(string userId, LimitStrategy? limit, CancellationToken cancellationToken);

    IAsyncEnumerable<ApplicationDto> SelectByRedirectAsync(string uri, LimitStrategy? limit, CancellationToken cancellationToken);

    Task<ApplicationDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);

    Task<long> CountAsync(CancellationToken cancellationToken);

    Task<int> InsertAsync(ApplicationDto application, CancellationToken cancellationToken);

    Task<int> UpdateAsync(string id, ApplicationDto application, CancellationToken cancellationToken);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect data to the application.
    /// </summary>
    /// <param name="redirect"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddRedirectAsync(RedirectDto redirect, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the specified redirect from the application.
    /// </summary>
    /// <param name="redirectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveRedirectAsync(string redirectId, CancellationToken cancellationToken);

    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}