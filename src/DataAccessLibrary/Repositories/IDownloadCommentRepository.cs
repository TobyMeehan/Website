using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for download comments.
/// </summary>
public interface IDownloadCommentRepository
{
    // SELECT

    /// <summary>
    /// Selects the comments of the specified download.
    /// </summary>
    /// <param name="downloadId"></param>
    /// <returns></returns>
    Task<List<DownloadCommentData>> SelectByDownloadAsync(string downloadId);

    /// <summary>
    /// Selects the comment with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DownloadCommentData> SelectByIdAsync(string id);
    
    // INSERT

    /// <summary>
    /// Inserts the specified comment data.
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    Task InsertAsync(DownloadCommentData comment);
    
    // UPDATE

    /// <summary>
    /// Updates the specified comment.
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    Task UpdateAsync(DownloadCommentData comment);
    
    // DELETE

    /// <summary>
    /// Deletes the specified comment.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string id);
}