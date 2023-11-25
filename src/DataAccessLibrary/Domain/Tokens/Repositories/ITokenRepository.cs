using TobyMeehan.Com.Data.Domain.Tokens.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Tokens.Repositories;

public interface ITokenRepository
{
    IAsyncEnumerable<TokenDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    
    IAsyncEnumerable<TokenDto> SelectByApplicationAsync(string applicationId, LimitStrategy? limit, CancellationToken cancellationToken);

    IAsyncEnumerable<TokenDto> SelectBySubjectAsync(string subject, LimitStrategy? limit, CancellationToken cancellationToken);

    IAsyncEnumerable<TokenDto> SelectByApplicationAndSubjectAsync(string applicationId, string? subject, LimitStrategy? limit,
        CancellationToken cancellationToken);

    IAsyncEnumerable<TokenDto> SelectByAuthorizationAsync(string authorizationId, LimitStrategy? limit, CancellationToken cancellationToken);

    Task<TokenDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    
    Task<TokenDto?> SelectByReferenceIdAsync(string referenceId, CancellationToken cancellationToken);

    Task<long> CountAsync(CancellationToken cancellationToken);
    
    Task<int> InsertAsync(TokenDto token, CancellationToken cancellationToken);
    
    Task<int> UpdateAsync(string id, TokenDto data, CancellationToken cancellationToken);
    
    Task DeleteByExpirationAsync(DateTime threshold, CancellationToken cancellationToken);
    
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}