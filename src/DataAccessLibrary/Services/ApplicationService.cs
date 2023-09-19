using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class ApplicationService : BaseService<IApplication, ApplicationData, CreateApplicationBuilder>, IApplicationService
{
    private readonly IApplicationRepository _db;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public ApplicationService(IApplicationRepository db, IIdService id, IPasswordService password, IMemoryCache cache) : base(db, cache)
    {
        _db = db;
        _id = id;
        _password = password;
    }

    protected override Task<IApplication> MapAsync(ApplicationData data)
    {
        var redirects = EntityCollection<IRedirect>.Create(data.Redirects, r =>
        {
            if (Uri.TryCreate(r.Uri, UriKind.Absolute, out var uri))
            {
                return new Redirect(r.Id, data.Id, uri);
            }

            throw new Exception($"Invalid redirect uri: {r.Uri}");
        });
        
        return Task.FromResult<IApplication>(new Application(data.Id, data.AuthorId, data.DownloadId, data.Name,
            data.Description, null, redirects));
    }
    
    protected override async Task<ApplicationData> CreateAsync(CreateApplicationBuilder create)
    {
        var id = await _id.GenerateAsync<IApplication>();

        return new ApplicationData
        {
            Id = id.Value,
            AuthorId = create.Author.Value,
            Name = create.Name
        };
    }
    
    public async Task<IApplication?> FindByCredentialsAsync(string id, Password secret, CancellationToken ct)
    {
        var application = await _db.SelectByIdAsync(id, ct);

        if (application is null)
        {
            return null;
        }

        return application.SecretHash switch
        {
            { } hash when await _password.CheckAsync(secret, hash) => await MapAsync(application),
            _ => null
        };
    }

    public async Task<IEntityCollection<IApplication>> GetByAuthorAsync(Id<IUser> user, CancellationToken ct)
    {
        var data = await _db.SelectByAuthorAsync(user.Value, ct);

        return await GetAsync(data);
    }

    public async Task<IApplication> UpdateAsync(Id<IApplication> id, UpdateApplicationBuilder update,
        CancellationToken ct)
    {
        return await UpdateAsync(id, data =>
        {
            data.Name = update.Name | data.Name;
            data.Description = update.Description | data.Description;
            data.DownloadId = update.Download.IsChanged ? update.Download.Value.Value.Value : data.DownloadId;
        }, ct);
    }

    public async Task<IRedirect> AddRedirectAsync(Id<IApplication> id, Uri uri, CancellationToken ct)
    {
        var redirectId = await _id.GenerateAsync<IRedirect>();

        await _db.AddRedirectAsync(new RedirectData
        {
            Id = redirectId.Value,
            ApplicationId = id.Value,
            Uri = uri.OriginalString
        }, ct);

        return new Redirect(redirectId.Value, id.Value, uri);
    }

    public async Task RemoveRedirectAsync(Id<IRedirect> redirect, CancellationToken ct)
    {
        await _db.RemoveRedirectAsync(redirect.Value, ct);
    }
}
