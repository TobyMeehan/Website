using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Storage;
using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.Avatar;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class AvatarService : BaseService<IAvatar, AvatarDto>, IAvatarService
{
    private readonly IAvatarRepository _db;
    private readonly IIdService _id;

    public AvatarService(
        IAvatarRepository db,
        IIdService id,
        ICacheService<AvatarDto, Id<IAvatar>> cache
        ) : base(cache)
    {
        _db = db;
        _id = id;
    }

    protected override async Task<IAvatar> MapAsync(AvatarDto data)
    {
        return new Avatar
        {
            Id = new Id<IAvatar>(data.Id),
            Filename = data.Filename,
            ContentType = MediaType.Parse(data.ContentType),
            Size = data.Size
        };
    }

    public async Task<OneOf<IAvatar, NotFound>> GetByIdAndUserAsync(Id<IAvatar> id, Id<IUser> userId,
        QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (data.UserId != userId.Value)
        {
            return new NotFound();
        }

        return await GetAsync<Avatar>(data);
    }

    public IAsyncEnumerable<IAvatar> GetByUserAsync(Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByUserAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task DownloadAsync(IAvatar avatar, Stream destination,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(avatar.Id) ?? await _db.SelectByIdAsync(avatar.Id.Value, cancellationToken);

        if (data is null)
        {
            throw new ArgumentException("Specified avatar does not exist.", nameof(avatar));
        }

        await DownloadFileAsync(data.ObjectName, destination, cancellationToken);
    }

    protected virtual Task DownloadFileAsync(Guid objectName, Stream destination, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }
    
    public async Task<IAvatar> CreateAsync(ICreateAvatar avatar, CancellationToken cancellationToken = default)
    {
        var storageObject = await UploadFileAsync(avatar.File, cancellationToken);
        
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

        await _db.InsertAsync(data, cancellationToken);

        return await GetAsync<Avatar>(data);
    }
    
    protected virtual Task<StorageObject> UploadFileAsync(IFileUpload avatar, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }

    public async Task DeleteByUserAsync(Id<IUser> user, CancellationToken cancellationToken = default)
    {
        await foreach (var avatar in _db.SelectByUserAsync(user.Value, null, cancellationToken))
        {
            await DeleteFileAsync(avatar.ObjectName, cancellationToken);
        }
        
        await _db.DeleteByUserAsync(user.Value, cancellationToken);
        
        Cache.RemoveWhere(x => x.UserId == user.Value);
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IAvatar> id, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        await DeleteFileAsync(data.ObjectName, cancellationToken);
        
        await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return new Success();
    }

    protected virtual Task DeleteFileAsync(Guid objectName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }
}