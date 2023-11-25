using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Applications;
using TobyMeehan.Com.Data.Domain.Applications.Models;
using TobyMeehan.Com.Data.Domain.Applications.Repositories;
using TobyMeehan.Com.Data.Security.Passwords;
using TobyMeehan.Com.Data.Storage.Configuration;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Data.Storage;

public class StorageEnabledApplicationService : ApplicationService
{
    private readonly IStorageService _storage;
    private readonly ApplicationIconStorageOptions _storageOptions;

    public StorageEnabledApplicationService(
        IApplicationRepository db, 
        IIconRepository iconRepo, 
        IIdService id, 
        IPasswordService password, 
        IStorageService storage,
        IOptions<StorageOptions> storageOptions,
        ICacheService<ApplicationDto, Id<IApplication>> cache) : base(db, iconRepo, id, password, cache)
    {
        _storage = storage;
        _storageOptions = storageOptions.Value.ApplicationIcons;
    }

    protected override async Task<StorageObject> UploadIconAsync(IFileUpload file, CancellationToken cancellationToken)
    {
        return await _storage.UploadAsync(_storageOptions.Bucket, file.ContentType, file.Stream, cancellationToken);
    }

    protected override async Task DownloadIconAsync(Guid objectName, Stream destination, CancellationToken cancellationToken)
    {
        await _storage.DownloadAsync(_storageOptions.Bucket, objectName, destination, cancellationToken);
    }

    protected override async Task DeleteIconAsync(Guid objectName, CancellationToken cancellationToken)
    {
        await _storage.DeleteAsync(_storageOptions.Bucket, objectName, cancellationToken);
    }
}