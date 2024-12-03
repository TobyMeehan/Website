using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.CloudStorage;

public interface IStorageService
{
    Task<string> SignSingleUploadAsync(string prefix, string key, string contentType);
    Task<UploadDto?> CreateMultipartUploadAsync(string prefix, string key, string contentType,
        CancellationToken cancellationToken);
    Task<string> SignUploadPartAsync(string prefix, string key, string uploadId, int partNumber);
    Task<IReadOnlyList<UploadPartDto>> GetUploadPartsAsync(string prefix, string key, string uploadId,
        CancellationToken cancellationToken);
    Task AbortMultipartUploadAsync(string prefix, string key, string uploadId, CancellationToken cancellationToken);
    Task CompleteMultipartUploadAsync(string prefix, string key, string uploadId,
        IEnumerable<UploadPartDto> parts,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string prefix, string key, CancellationToken cancellationToken);
}