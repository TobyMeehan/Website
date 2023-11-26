using System.Transactions;
using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Downloads.Models;
using TobyMeehan.Com.Data.Domain.Downloads.Repositories;
using TobyMeehan.Com.Models.Download;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Downloads;

public class DownloadService : BaseService<Download, IDownload, DownloadDto>, IDownloadService
{
    private readonly IDownloadRepository _db;
    private readonly IDownloadAuthorRepository _authorRepo;
    private readonly IIdService _id;

    public DownloadService(
        IDownloadRepository db,
        IDownloadAuthorRepository authorRepo,
        IIdService id,
        ICacheService<DownloadDto, Id<IDownload>> cache) : base(cache)
    {
        _db = db;
        _authorRepo = authorRepo;
        _id = id;
    }

    private IDownloadAuthor MapAuthor(AuthorDto data)
    {
        return new Author
        {
            Id = new Id<IUser>(data.UserId),
            DownloadId = new Id<IDownload>(data.DownloadId),
            Username = data.Username,
            DisplayName = data.DisplayName,
            CanEdit = data.CanEdit,
            CanManageAuthors = data.CanManageAuthors,
            CanManageFiles = data.CanManageFiles,
            CanDelete = data.CanDelete
        };
    }
    
    protected override async Task<Download> MapAsync(DownloadDto data)
    {
        var download = new Download
        {
            Id = new Id<IDownload>(data.Id),
            Title = data.Title,
            Summary = data.Summary,
            Description = data.Description,
            Verification = data.Verification,
            Visibility = data.Visibility,
            UpdatedAt = data.UpdatedAt,
            Authors = new EntityCollection<IDownloadAuthor, IUser>(data.Authors.Select(MapAuthor))
        };

        return download;
    }

    public IAsyncEnumerable<IDownload> GetPublicAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectPublicAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IDownload> GetByAuthorAsync(Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByAuthorAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IDownload, NotFound>> GetByIdAsync(Id<IDownload> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync(data);
    }

    public async Task<IDownload> CreateAsync(ICreateDownload create, CancellationToken cancellationToken = default)
    {
        var id = await _id.GenerateAsync<IDownload>();

        var data = new DownloadDto
        {
            Id = id.Value,
            Title = create.Title,
            Summary = create.Summary,
            Description = create.Description,
            Verification = VerificationNames.None,
            Visibility = create.Visibility,
            UpdatedAt = DateTime.UtcNow
        };

        var authorData = new AuthorDto
        {
            DownloadId = id.Value,
            UserId = create.User.Value,
            CanEdit = true,
            CanManageAuthors = true,
            CanManageFiles = true,
            CanDelete = true
        };
        
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _db.InsertAsync(data, cancellationToken);
            await _authorRepo.InsertAsync(authorData, cancellationToken);
            
            scope.Complete();
        }

        return await GetAsync(data);
    }

    public async Task<OneOf<IDownload, NotFound>> UpdateAsync(Id<IDownload> id, IUpdateDownload update, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        data.Title = update.Title | data.Title;
        data.Summary = update.Summary | data.Summary;
        data.Description = update.Description | data.Description;
        
        data.Verification = update.Verification | data.Verification;
        data.Visibility = update.Visibility | data.Visibility;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IDownload> id, CancellationToken cancellationToken)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        return result > 0 ? new Success() : new NotFound();
    }
}