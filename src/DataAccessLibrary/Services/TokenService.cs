using TobyMeehan.Com.Builders.Token;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Exceptions;
using TobyMeehan.Com.Models.Token;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

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
        return new Token
        {
            Id = new Id<IToken>(data.Id),
            Authorization = await _authorizations.FindByIdAsync(data.AuthorizationId),
            Payload = data.Payload,
            ReferenceId = data.ReferenceId,
            Status = data.Status,
            Type = data.Type,
            RedemptionDate = data.RedeemedAt,
            ExpiresAt = data.ExpiresAt,
            CreatedAt = data.CreatedAt
        };
    }

    public IAsyncEnumerable<IToken> FindByApplicationAsync(string applicationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAsync(applicationId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> FindByUserAsync(string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByUserAsync(userId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> FindByApplicationAndUserAsync(string applicationId, string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAndUserAsync(applicationId, userId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IToken> FindByAuthorizationAsync(string authorizationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByAuthorizationAsync(authorizationId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<IToken?> FindByIdAsync(string id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<IToken?> FindByReferenceIdAsync(string referenceId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByReferenceIdAsync(referenceId, cancellationToken);

        return await GetAsync(data);
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
        var id = await _id.GenerateAsync<IToken>();

        var data = new TokenDto
        {
            Id = id.Value,
            AuthorizationId = create.Authorization.Value,
            ReferenceId = create.ReferenceId,
            Payload = create.Payload,
            Status = create.Status,
            Type = create.Type,
            RedeemedAt = create.RedeemedAt,
            ExpiresAt = create.ExpiresAt,
            CreatedAt = create.CreatedAt
        };

        await _db.InsertAsync(data, cancellationToken);

        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task<IToken> UpdateAsync(Id<IToken> id, IUpdateToken update, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<IToken>(id);
        }

        data.Payload = update.Payload | data.Payload;
        data.Status = update.Status | data.Status;
        data.RedeemedAt = update.RedeemedAt | data.RedeemedAt;
        data.ExpiresAt = update.ExpiresAt | data.ExpiresAt;

        await _db.UpdateAsync(id.Value, data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task DeleteAsync(Id<IToken> id, CancellationToken cancellationToken = default)
    {
        await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);
    }

    public async Task DeleteByExpirationAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        await _db.DeleteByExpirationAsync(threshold, cancellationToken);

        Cache.RemoveWhere(x => x.ExpiresAt < threshold);
    }
}