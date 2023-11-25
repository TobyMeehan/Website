using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Tokens.Models;
using TobyMeehan.Com.Data.Domain.Tokens.Repositories;
using TobyMeehan.Com.Models.Token;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Tokens;

public class TokenService : BaseService<IToken, TokenDto>, ITokenService
{
    private readonly ITokenRepository _db;
    private readonly IAuthorizationService _authorizations;
    private readonly IIdService _id;

    public TokenService(ITokenRepository db, IAuthorizationService authorizations, IIdService id, ICacheService<TokenDto, Id<IToken>> cache) : base(cache)
    {
        _db = db;
        _authorizations = authorizations;
        _id = id;
    }

    protected override async Task<IToken> MapAsync(TokenDto data)
    {
        var authorization = await _authorizations.GetByIdAsync(new Id<IAuthorization>(data.AuthorizationId));
        
        return new Token
        {
            Id = new Id<IToken>(data.Id),
            Authorization = authorization.Match<IAuthorization?>(
                result => result,
                _ => null),
            Payload = data.Payload,
            ReferenceId = data.ReferenceId,
            Status = data.Status,
            Type = data.Type,
            Subject = data.Subject,
            RedemptionDate = data.RedeemedAt,
            ExpiresAt = data.ExpiresAt,
            CreatedAt = data.CreatedAt
        };
    }

    public IAsyncEnumerable<IToken> GetByApplicationAsync(Id<IApplication> application, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAsync(application.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> GetBySubjectAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectBySubjectAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> GetByApplicationAndSubjectAsync(Id<IApplication> application, string? subject,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAndSubjectAsync(application.Value, subject, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> GetByAuthorizationAsync(Id<IAuthorization> authorization,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByAuthorizationAsync(authorization.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IToken, NotFound>> GetByIdAsync(Id<IToken> id, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }
        
        return OneOf<IToken, NotFound>.FromT0(
            await GetAsync(data));
    }

    public async Task<OneOf<IToken, NotFound>> GetByReferenceIdAsync(string referenceId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByReferenceIdAsync(referenceId, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }
        
        return OneOf<IToken, NotFound>.FromT0(
            await GetAsync(data));
    }

    public IAsyncEnumerable<IToken> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CountAsync(cancellationToken);
    }

    public async Task<IToken> CreateAsync(ICreateToken create, CancellationToken cancellationToken = default)
    {
        var id = create.Id | await _id.GenerateAsync<IToken>();

        var data = new TokenDto
        {
            Id = id.Value,
            AuthorizationId = create.Authorization.Value,
            ReferenceId = create.ReferenceId,
            Payload = create.Payload,
            Status = create.Status,
            Type = create.Type,
            Subject = create.Subject,
            RedeemedAt = create.RedeemedAt,
            ExpiresAt = create.ExpiresAt,
            CreatedAt = create.CreatedAt
        };

        await _db.InsertAsync(data, cancellationToken);

        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task<OneOf<IToken, NotFound>> UpdateAsync(Id<IToken> id, IUpdateToken update,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        data.ReferenceId = update.ReferenceId | data.ReferenceId;
        data.Payload = update.Payload | data.Payload;
        data.Status = update.Status | data.Status;
        data.RedeemedAt = update.RedeemedAt | data.RedeemedAt;
        data.ExpiresAt = update.ExpiresAt | data.ExpiresAt;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return OneOf<IToken, NotFound>.FromT0(
            await GetAsync(data));
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IToken> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task DeleteByExpirationAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        await _db.DeleteByExpirationAsync(threshold, cancellationToken);

        Cache.RemoveWhere(x => x.ExpiresAt < threshold);
    }
}