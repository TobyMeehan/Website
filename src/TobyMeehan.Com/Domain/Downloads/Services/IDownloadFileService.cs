namespace TobyMeehan.Com.Domain.Downloads.Services;

public record CreateDownloadFile(Guid DownloadId, string Filename, string ContentType, long SizeInBytes);
public record UpdateDownloadFile(string Filename);

public interface IDownloadFileService
{
    Task<(DownloadFile File, string UploadUrl)> CreateAsync(CreateDownloadFile create,
        CancellationToken cancellationToken = default);
    
    Task<FileUpload?> CreateUploadAsync(DownloadFile file, CancellationToken cancellationToken = default);
    
    Task CompleteUploadAsync(DownloadFile file, CancellationToken cancellationToken = default);
    
    Task<string> SignUploadPartAsync(DownloadFile file, string uploadId, int partNumber,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<FileUploadPart>> GetUploadPartsAsync(DownloadFile file, string uploadId,
        CancellationToken cancellationToken = default);
    
    Task CompleteUploadAsync(DownloadFile file, string uploadId, IEnumerable<FileUploadPart> parts,
        CancellationToken cancellationToken = default);
    
    Task DeleteUploadAsync(DownloadFile file, string uploadId,
        CancellationToken cancellationToken = default);

    Task<string> CreateDownloadAsync(DownloadFile file, Guid? userId,
        CancellationToken cancellationToken = default);
    
    Task<DownloadFile?> GetByIdAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken = default);
    
    Task<DownloadFile?> GetByFilenameAsync(Guid downloadId, string filename, 
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DownloadFile>> GetByDownloadAsync(Guid downloadId, 
        CancellationToken cancellationToken = default);

    Task<DownloadFile?> UpdateAsync(Guid downloadId, Guid fileId, UpdateDownloadFile update,
        CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken = default);
}
