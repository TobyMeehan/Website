using System.Transactions;
using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Applications.Models;
using TobyMeehan.Com.Data.Domain.Applications.Repositories;
using TobyMeehan.Com.Data.Security.Passwords;
using TobyMeehan.Com.Data.Storage;
using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.Application;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Applications;

public class ApplicationService : BaseService<Application, IApplication, ApplicationDto>, IApplicationService
{
    private readonly IApplicationRepository _db;
    private readonly IIconRepository _iconRepo;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public ApplicationService(
        IApplicationRepository db,
        IIconRepository iconRepo,
        IIdService id,
        IPasswordService password,
        ICacheService<ApplicationDto, Id<IApplication>> cache) : base(cache)
    {
        _db = db;
        _iconRepo = iconRepo;
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

    protected override Task<Application> MapAsync(ApplicationDto dto)
    {
        var id = new Id<IApplication>(dto.Id);

        var redirects = EntityCollection<IRedirect>.Create(dto.Redirects, MapRedirect);

        var application = new Application
        {
            Id = id,
            AuthorId = new Id<IUser>(dto.AuthorId),
            DownloadId = dto.DownloadId is null ? null : new Id<IDownload>(dto.DownloadId),
            Name = dto.Name,
            Description = dto.Description,
            HasSecret = dto.Secret is not null,
            Redirects = redirects
        };

        if (dto.Icon is not null)
        {
            application.Icon = new Icon
            {
                Filename = dto.Icon.Filename,
                ContentType = MediaType.Parse(dto.Icon.ContentType),
                Size = dto.Icon.Size
            };
        }
        
        return Task.FromResult(application);
    }

    public async Task<OneOf<IApplication, NotFound>> GetByIdAndAuthorAsync(Id<IApplication> id, Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (data.AuthorId != user.Value)
        {
            return new NotFound();
        }

        return await GetAsync(data);
    }

    public async Task<OneOf<IApplication, InvalidCredentials, NotFound>> GetByCredentialsAsync(Id<IApplication> id,
        Password secret,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (data.Secret is { } secretHash)
            switch (await _password.CheckAsync(secret, secretHash))
            {
                case { Succeeded: true, Rehash: { HasValue: true } rehash }:
                    data.Secret = rehash.Value;
                    await _db.UpdateAsync(data.Id, data, cancellationToken);
                    break;

                case { Succeeded: false }:
                    return new InvalidCredentials();
            }

        return await GetAsync(data);
    }

    public IAsyncEnumerable<IApplication> GetAllAsync(QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IApplication> GetByAuthorAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
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

    public async Task<OneOf<IApplication, NotFound>> GetByIdAsync(Id<IApplication> id, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync(data);
    }

    public async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CountAsync(cancellationToken);
    }

    public async Task DownloadIconAsync(IApplication application, Stream destination,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(application.Id.Value, cancellationToken);

        if (data?.Icon is null)
        {
            throw new ArgumentException("Specified application does not exist or has not set an icon.");
        }

        await DownloadIconAsync(data.Icon.ObjectName, destination, cancellationToken);
    }

    protected virtual Task DownloadIconAsync(Guid objectName, Stream destination, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }

    public async Task<IApplication> CreateAsync(ICreateApplication application,
        CancellationToken cancellationToken = default)
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

        if (application.Icon.HasValue)
            switch (application.Icon.Value)
            {
                case null when data.Icon is not null:
                    await DeleteIconAsync(data.Icon.ObjectName, cancellationToken);
                    await _iconRepo.DeleteByApplicationAsync(data.Id, cancellationToken);
                    break;

                case { } fileUpload:
                    var storageObject = await UploadIconAsync(fileUpload, cancellationToken);

                    if (data.Icon is not null)
                    {
                        await DeleteIconAsync(data.Icon.ObjectName, cancellationToken);
                    }

                    data.Icon = new IconDto
                    {
                        ApplicationId = data.Id,
                        ObjectName = storageObject.ObjectName,
                        Filename = fileUpload.Filename,
                        ContentType = fileUpload.ContentType.ToString(),
                        Size = fileUpload.Size
                    };

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _iconRepo.DeleteByApplicationAsync(data.Id, cancellationToken);
                        await _iconRepo.InsertAsync(data.Icon, cancellationToken);

                        scope.Complete();
                    }

                    break;
            }

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        Cache.Remove(id);

        return await GetAsync(data);
    }

    protected virtual Task<StorageObject> UploadIconAsync(IFileUpload file, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }

    protected virtual Task DeleteIconAsync(Guid objectName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cloud storage is not enabled.");
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IApplication> id,
        CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);

        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task<OneOf<IRedirect, NotFound>> AddRedirectAsync(Id<IApplication> id, Uri uri,
        CancellationToken cancellationToken = default)
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