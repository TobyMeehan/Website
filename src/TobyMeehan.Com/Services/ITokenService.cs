using OneOf;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Token;
using TobyMeehan.Com.Models.Token;

namespace TobyMeehan.Com.Services;

public interface ITokenService
{
    IAsyncEnumerable<IToken> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<IToken> GetByApplicationAsync(Id<IApplication> application, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> GetBySubjectAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> GetByApplicationAndSubjectAsync(Id<IApplication> application, string? subject,
        QueryOptions? options = null, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IToken> GetByAuthorizationAsync(Id<IAuthorization> authorization, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    Task<OneOf<IToken, NotFound>> GetByIdAsync(Id<IToken> id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<OneOf<IToken, NotFound>> GetByReferenceIdAsync(string referenceId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<IToken> CreateAsync(ICreateToken create, CancellationToken cancellationToken = default);

    Task<OneOf<IToken, NotFound>> UpdateAsync(Id<IToken> id, IUpdateToken update, CancellationToken cancellationToken = default);
    
    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IToken> id, CancellationToken cancellationToken = default);

    Task DeleteByExpirationAsync(DateTime threshold, CancellationToken cancellationToken = default);
}