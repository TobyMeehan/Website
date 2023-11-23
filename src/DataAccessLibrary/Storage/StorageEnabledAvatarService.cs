using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;
using TobyMeehan.Com.Models.Avatar;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Storage;

public class StorageEnabledAvatarService : AvatarService, IAvatarService
{
    private readonly IIdService _id;
    private readonly IStorageService _storage;
    private readonly StorageOptions _storageOptions;

    public StorageEnabledAvatarService(
        IAvatarRepository db, 
        IIdService id, 
        IStorageService storage, 
        IOptions<StorageOptions> storageOptions, 
        ICacheService<AvatarDto, Id<IAvatar>> cache) : base(db, cache)
    {
        _id = id;
        _storage = storage;
        _storageOptions = storageOptions.Value;
    }
    
    public override async Task<IAvatar> CreateAsync(ICreateAvatar avatar, CancellationToken cancellationToken = default)
    {
        var storageObject = await _storage.UploadAsync(_storageOptions.Avatars.Bucket, avatar.File.ContentType,
            avatar.File.Stream, cancellationToken);

        var id = await _id.GenerateAsync<IAvatar>();

        var data = new AvatarDto
        {
            Id = id.Value,
            UserId = avatar.User.Value,
            ObjectName = storageObject.ObjectName,
            Filename = $"{id.Value}{avatar.File.ContentType.Extension}",
            ContentType = avatar.File.ContentType.ToString(),
            Size = avatar.File.Size
        };

        await InsertAsync(data, cancellationToken);

        return await GetAsync<Avatar>(data);
    }
}