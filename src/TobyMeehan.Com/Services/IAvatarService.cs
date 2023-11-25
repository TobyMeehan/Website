using OneOf;
using TobyMeehan.Com.Models.Avatar;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Services;

public interface IAvatarService
{
    Task<OneOf<IAvatar, NotFound>> GetByIdAndUserAsync(Id<IAvatar> id, Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<IAvatar> GetByUserAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    Task DownloadAsync(IAvatar avatar, Stream destination, CancellationToken cancellationToken = default);

    Task<IAvatar> CreateAsync(ICreateAvatar avatar, CancellationToken cancellationToken = default);
    Task DeleteByUserAsync(Id<IUser> user, CancellationToken cancellationToken = default);
    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IAvatar> id, CancellationToken cancellationToken = default);
}