using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class ApplicationService : BaseService<IApplication, ApplicationData, CreateApplicationBuilder>, IApplicationService
{
    private readonly IApplicationRepository _db;
    private readonly IIdService _id;

    public ApplicationService(IApplicationRepository db, IIdService id) : base(db)
    {
        _db = db;
        _id = id;
    }

    protected override Task<IApplication> MapperAsync(ApplicationData data)
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
    
    protected override async Task<(Id<IApplication>, ApplicationData)> CreateAsync(CreateApplicationBuilder create)
    {
        var id = await _id.GenerateAsync<IApplication>();

        return (id, new ApplicationData
        {
            Id = id.Value,
            AuthorId = create.Author.Value,
            Name = create.Name
        });
    }
    
    public async Task<IEntityCollection<IApplication>> GetByAuthorAsync(Id<IUser> user, CancellationToken ct)
    {
        var data = await _db.SelectByAuthorAsync(user.Value, ct);

        return await MapAsync(data);
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
            Uri = uri.AbsoluteUri
        }, ct);

        return new Redirect(redirectId.Value, id.Value, uri);
    }

    public async Task RemoveRedirectAsync(Id<IRedirect> redirect, CancellationToken ct)
    {
        await _db.RemoveRedirectAsync(redirect.Value, ct);
    }
}
