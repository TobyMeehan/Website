using TobyMeehan.Com.Data.Domain.Avatars.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Avatars.Repositories;

public interface IAvatarRepository
{
    IAsyncEnumerable<AvatarDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken cancellationToken);
    Task<AvatarDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    Task<int> InsertAsync(AvatarDto data, CancellationToken cancellationToken);
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<int> DeleteByUserAsync(string userId, CancellationToken cancellationToken);
}