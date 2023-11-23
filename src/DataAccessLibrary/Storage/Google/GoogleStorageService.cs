using Google.Cloud.Storage.V1;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Data.Storage.Google;

public class GoogleStorageService : IStorageService
{
    private readonly StorageClient _client;

    public GoogleStorageService(StorageClient client)
    {
        _client = client;
    }
    
    public async Task<StorageObject> UploadAsync(string bucket, MediaType mediaType, Stream source,
        CancellationToken cancellationToken)
    {
        var objectName = Guid.NewGuid();
        
        var @object = await _client.UploadObjectAsync(bucket, objectName.ToString(), mediaType.ToString(), source, 
            cancellationToken: cancellationToken);
        
        return new StorageObject
        {
            ObjectName = objectName,
            Size = unchecked((long) (@object.Size ?? 0))
        };
    }

    public async Task DeleteAsync(string bucket, Guid objectName, CancellationToken cancellationToken)
    {
        await _client.DeleteObjectAsync(bucket, objectName.ToString(), cancellationToken: cancellationToken);
    }
}