using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Models.Application;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class ApplicationService : BaseService<IApplication, ApplicationDto>, IApplicationService
{
    private readonly IApplicationRepository _db;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public ApplicationService(
        IApplicationRepository db, 
        IIdService id, 
        IPasswordService password, 
        ICacheService<ApplicationDto, Id<IApplication>> cache) : base(cache)
    {
        _db = db;
        _id = id;
        _password = password;
    }

    private static IRedirect MapRedirect(RedirectDto dto)
    {
        return new Redirect
        {
            Id = new Id<IRedirect>(dto.Id),
            ApplicationId = new Id<IApplication>(dto.ApplicationId),
            Uri = new Uri(dto.Uri)
        };
    }
    
    protected override Task<IApplication> MapAsync(ApplicationDto dto)
    {
        var id = new Id<IApplication>(dto.Id);
        
        var redirects = EntityCollection<IRedirect>.Create(dto.Redirects, MapRedirect);
        
        return Task.FromResult<IApplication>(new Application
        {
            Id = id,
            AuthorId = new Id<IUser>(dto.AuthorId),
            DownloadId = dto.DownloadId is null ? null : new Id<IDownload>(dto.DownloadId),
            Name = dto.Name,
            Description = dto.Description,
            Redirects = redirects
        });
    }

    public async Task<OneOf<IApplication, InvalidCredentials, NotFound>> GetByCredentialsAsync(Id<IApplication> id, Password secret,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data?.SecretHash is null)
        {
            return new NotFound();
        }

        if (!await _password.CheckAsync(secret, data.SecretHash))
        {
            return new InvalidCredentials();
        }

        return OneOf<IApplication, InvalidCredentials, NotFound>.FromT0(
            await GetAsync(data));
    }

    public IAsyncEnumerable<IApplication> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IApplication> GetByAuthorAsync(Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByAuthorAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IApplication> GetByRedirectAsync(string uri, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByRedirectAsync(uri, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IApplication, NotFound>> GetByIdAsync(Id<IApplication> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return OneOf<IApplication, NotFound>.FromT0(
            await GetAsync(data));
    }

    public async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CountAsync(cancellationToken);
    }

    public async Task<IApplication> CreateAsync(ICreateApplication application, CancellationToken cancellationToken = default)
    {
        var id = await _id.GenerateAsync<IApplication>();

        var data = new ApplicationDto
        {
            Id = id.Value,
            AuthorId = application.Author.Value,
            Name = application.Name
        };

        await _db.InsertAsync(data, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<OneOf<IApplication, NotFound>> UpdateAsync(Id<IApplication> id, IUpdateApplication application,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        data.DownloadId = application.Download.MapOr(x => x?.Value, data.DownloadId);
        data.Name = application.Name | data.Name;
        data.Description = application.Description | data.Description;
        // TODO: Icon

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return OneOf<IApplication, NotFound>.FromT0(
            await GetAsync(data));
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IApplication> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task<OneOf<IRedirect, NotFound>> AddRedirectAsync(Id<IApplication> id, Uri uri, CancellationToken cancellationToken = default)
    {
        var redirectId = await _id.GenerateAsync<IRedirect>();

        var data = new RedirectDto
        {
            Id = redirectId.Value,
            ApplicationId = id.Value,
            Uri = uri.OriginalString
        };
        
        await _db.AddRedirectAsync(data, cancellationToken);

        Cache.Remove(id);
        
        return OneOf<IRedirect, NotFound>.FromT0(
            MapRedirect(data));
    }

    public async Task RemoveRedirectAsync(Id<IRedirect> redirect, CancellationToken cancellationToken = default)
    {
        await _db.RemoveRedirectAsync(redirect.Value, cancellationToken);
        
        Cache.RemoveWhere(x => x.Redirects.Any(y => y.Id == redirect.Value));
    }
}
