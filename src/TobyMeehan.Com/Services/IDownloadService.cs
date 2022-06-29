using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for download data.
/// </summary>
public interface IDownloadService
{
    // GET

    /// <summary>
    /// Gets all downloads.
    /// </summary>
    /// <returns></returns>
    Task<IEntityCollection<IDownload>> GetAllAsync();

    /// <summary>
    /// Gets all downloads where the specified user is an author.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="includeUnlisted"></param>
    /// <returns></returns>
    Task<IEntityCollection<IDownload>> GetByUserAsync(Id<IUser> user, bool includeUnlisted = false);

    /// <summary>
    /// Gets the download with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDownload> GetByIdAsync(Id<IDownload> id);
    
    // CREATE

    /// <summary>
    /// Creates a new download with the specified builder.
    /// </summary>
    /// <param name="download"></param>
    /// <returns></returns>
    Task<IDownload> CreateAsync(CreateDownloadBuilder download);
    
    // UPDATE

    /// <summary>
    /// Updates the specified download with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="download"></param>
    /// <returns></returns>
    Task<IDownload> UpdateAsync(Id<IDownload> id, UpdateDownloadBuilder download);
    
    // DELETE

    /// <summary>
    /// Deletes the specified download.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IDownload> id);
    
    // RELATIONS

    /// <summary>
    /// Adds the specified user as an author to the specified download.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IDownloadAuthor> AddAuthorAsync(Id<IDownload> id, Id<IUser> user);

    /// <summary>
    /// Removes the specified user as an author from the specified download.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="author"></param>
    /// <returns></returns>
    Task RemoveAuthorAsync(Id<IDownload> id, Id<IUser> author);
}