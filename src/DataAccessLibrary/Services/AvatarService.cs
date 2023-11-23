using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models.Avatar;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class AvatarService : BaseService<IAvatar, AvatarDto>, IAvatarService
{
    private readonly IAvatarRepository _db;

    public AvatarService(
        IAvatarRepository db,
        ICacheService<AvatarDto, Id<IAvatar>> cache
        ) : base(cache)
    {
        _db = db;
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

    public async Task<OneOf<IAvatar, NotFound>> GetByIdAsync(Id<IAvatar> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
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

    public virtual Task<IAvatar> CreateAsync(ICreateAvatar avatar, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }

    protected async Task<int> InsertAsync(AvatarDto data, CancellationToken cancellationToken)
    {
        return await _db.InsertAsync(data, cancellationToken);
    }

    public async Task DeleteByUserAsync(Id<IUser> user, CancellationToken cancellationToken = default)
    {
        await _db.DeleteByUserAsync(user.Value, cancellationToken);
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IAvatar> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);

        return result > 1 ? new Success() : new NotFound();
    }
}