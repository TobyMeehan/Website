using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for download comment data.
/// </summary>
public interface IDownloadCommentService
{
    // GET

    /// <summary>
    /// Gets all the comments for the specified download.
    /// </summary>
    /// <param name="download"></param>
    /// <returns></returns>
    Task<IEntityCollection<IDownloadComment>> GetByDownloadAsync(Id<IDownload> download);

    /// <summary>
    /// Gets the comment with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDownloadComment> GetByIdAsync(Id<IDownloadComment> id);
    
    // CREATE

    /// <summary>
    /// Creates a new comment with the specified builder.
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    Task<IDownloadComment> CreateAsync(CreateDownloadCommentBuilder comment);
    
    // UPDATE

    /// <summary>
    /// Updates the specified comment with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    Task<IDownloadComment> UpdateAsync(Id<IDownloadComment> id, UpdateDownloadCommentBuilder comment);
    
    // DELETE

    /// <summary>
    /// Deletes the specified comment.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IDownloadComment> id);
}