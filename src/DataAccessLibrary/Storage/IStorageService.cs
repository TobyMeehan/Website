using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Data.Storage;

public interface IStorageService
{
    Task<StorageObject> UploadAsync(string bucket, MediaType mediaType, Stream source,
        CancellationToken cancellationToken);

    Task DeleteAsync(string bucket, Guid objectName, CancellationToken cancellationToken);
}