using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Repositories;

public interface IAuthorizationRepository
{
    IAsyncEnumerable<AuthorizationDto> SelectByApplicationAsync(string applicationId, LimitStrategy? limit,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AuthorizationDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken cancellationToken);

    IAsyncEnumerable<AuthorizationDto> SelectByApplicationAndUserAsync(string applicationId, string userId, LimitStrategy? limit,
        CancellationToken cancellationToken);

    Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken);
    Task<AuthorizationDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    IAsyncEnumerable<AuthorizationDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    Task<long> CountAsync(CancellationToken cancellationToken);
    Task<int> InsertAsync(AuthorizationDto data, CancellationToken cancellationToken);
    Task<int> UpdateAsync(string id, AuthorizationDto data, CancellationToken cancellationToken);
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}