using OneOf;
using TobyMeehan.Com.Models.Download;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Services;

public interface IDownloadService
{
    IAsyncEnumerable<IDownload> GetPublicAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IDownload> GetByAuthorAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    
    Task<OneOf<IDownload, NotFound>> GetByIdAsync(Id<IDownload> id, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<IDownload> CreateAsync(ICreateDownload create, CancellationToken cancellationToken = default);

    Task<OneOf<IDownload, NotFound>> UpdateAsync(Id<IDownload> id, IUpdateDownload update,
        CancellationToken cancellationToken = default);
    
    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IDownload> id, CancellationToken cancellationToken);
}