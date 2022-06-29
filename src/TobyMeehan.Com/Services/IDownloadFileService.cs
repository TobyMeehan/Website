using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for download file data.
/// </summary>
public interface IDownloadFileService
{
    // GET

    /// <summary>
    /// Gets all the files for the specified download.
    /// </summary>
    /// <param name="download"></param>
    /// <returns></returns>
    Task<IEntityCollection<IDownloadFile>> GetByDownloadAsync(Id<IDownload> download);

    /// <summary>
    /// Gets the file with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDownloadFile> GetByIdAsync(Id<IDownloadFile> id);

    /// <summary>
    /// Gets the file from the specified download with the specified filename.
    /// </summary>
    /// <param name="download"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    Task<IDownloadFile> GetByDownloadAndFilenameAsync(Id<IDownload> download, string filename);
    
    // CREATE

    /// <summary>
    /// Creates a new download file with the specified builder.
    /// </summary>
    /// <param name="downloadFile"></param>
    /// <returns></returns>
    Task<IDownloadFile> CreateAsync(CreateDownloadFileBuilder downloadFile);
    
    // UPDATE
    
    // DELETE

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IDownloadFile> id);
}