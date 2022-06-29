using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for downloads.
/// </summary>
public interface IDownloadRepository
{
    // SELECT
    
    /// <summary>
    /// Selects all public downloads.
    /// </summary>
    /// <returns></returns>
    Task<List<DownloadData>> SelectAllAsync();

    /// <summary>
    /// Selects all downloads where the specified user is an author.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="includeUnlisted"></param>
    /// <returns></returns>
    Task<List<DownloadData>> SelectByUserAsync(string userId, bool includeUnlisted);

    /// <summary>
    /// Selects the download with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DownloadData> SelectByIdAsync(string id);
    
    // INSERT

    /// <summary>
    /// Inserts the specified download data.
    /// </summary>
    /// <param name="download"></param>
    /// <returns></returns>
    Task InsertAsync(DownloadData download);
    
    // UPDATE

    /// <summary>
    /// Updates the specified download.
    /// </summary>
    /// <param name="download"></param>
    /// <returns></returns>
    Task UpdateAsync(DownloadData download);
    
    // DELETE

    /// <summary>
    /// Deletes the specified download.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string id);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified user as an author.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AddAuthorAsync(string id, string userId);

    /// <summary>
    /// Removes the specified user as an author.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RemoveAuthorAsync(string id, string userId);
}