using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Domain.Avatars;
using TobyMeehan.Com.Data.Domain.Avatars.Models;
using TobyMeehan.Com.Data.Domain.Avatars.Repositories;
using TobyMeehan.Com.Data.Storage.Configuration;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Data.Storage;

public class StorageEnabledAvatarService : AvatarService
{
    private readonly IStorageService _storage;
    private readonly StorageOptions _storageOptions;
    
    public StorageEnabledAvatarService(
        IAvatarRepository db,
        IIdService id,
        IStorageService storage,
        IOptions<StorageOptions> storageOptions,
        ICacheService<AvatarDto, Id<IAvatar>> cache) : base(db, id, cache)
    {
        _storage = storage;
        _storageOptions = storageOptions.Value;
    }

    protected override async Task<StorageObject> UploadFileAsync(IFileUpload file, CancellationToken cancellationToken)
    {
        return await _storage.UploadAsync(_storageOptions.Avatars.Bucket, file.ContentType,
            file.Stream, cancellationToken);
    }

    protected override async Task DownloadFileAsync(Guid objectName, Stream destination, CancellationToken cancellationToken)
    {
        await _storage.DownloadAsync(_storageOptions.Avatars.Bucket, objectName, destination,
            cancellationToken: cancellationToken);
    }

    protected override async Task DeleteFileAsync(Guid objectName, CancellationToken cancellationToken)
    {
        await _storage.DeleteAsync(_storageOptions.Avatars.Bucket, objectName, cancellationToken);
    }
}