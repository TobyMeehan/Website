using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for applications.
/// </summary>
public interface IApplicationRepository
{
    // SELECT

    /// <summary>
    /// Selects all applications with the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<ApplicationData>> SelectByUserAsync(string userId);

    /// <summary>
    /// Selects the application with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ApplicationData> SelectByIdAsync(string id);
    
    // INSERT

    /// <summary>
    /// Inserts the specified application data.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    Task InsertAsync(ApplicationData application);
    
    // UPDATE

    /// <summary>
    /// Updates the specified application.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    Task UpdateAsync(ApplicationData application);
    
    // DELETE

    /// <summary>
    /// Deletes the specified application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string id);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified redirect data to the application.
    /// </summary>
    /// <param name="redirect"></param>
    /// <returns></returns>
    Task AddRedirectAsync(RedirectData redirect);

    /// <summary>
    /// Removes the specified redirect from the application.
    /// </summary>
    /// <param name="redirectId"></param>
    /// <returns></returns>
    Task RemoveRedirectAsync(string redirectId);
}