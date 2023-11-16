using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Token;
using TobyMeehan.Com.Models.Token;

namespace TobyMeehan.Com.Services;

public interface ITokenService
{
    IAsyncEnumerable<IToken> FindByApplicationAsync(string applicationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> FindByUserAsync(string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> FindByApplicationAndUserAsync(string applicationId, string userId,
        QueryOptions? options = null, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> FindByAuthorizationAsync(string authorizationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<IToken?> FindByIdAsync(string id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<IToken?> FindByReferenceIdAsync(string referenceId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<IToken> CreateAsync(ICreateToken create, CancellationToken cancellationToken = default);

    Task<IToken> UpdateAsync(Id<IToken> id, IUpdateToken update, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Id<IToken> id, CancellationToken cancellationToken = default);

    Task DeleteByExpirationAsync(DateTime threshold, CancellationToken cancellationToken = default);
}