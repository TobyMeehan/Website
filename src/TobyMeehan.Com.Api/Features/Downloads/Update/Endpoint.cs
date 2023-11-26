using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Download;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.Update;

public class Endpoint : Endpoint<Request, DownloadResponse>
{
    private readonly IDownloadService _service;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IDownloadService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Patch("/downloads/{DownloadId}");
        Policies(PolicyNames.Download.Scope.Update);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IDownload>(req.DownloadId), cancellationToken: ct);

        await result.Match(
            download => AuthorizeAsync(download, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IDownload download, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, PolicyNames.Download.Operation.Update);

        if (authorizationResult.Succeeded)
        {
            await UpdateAsync(download.Id, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task UpdateAsync(Id<IDownload> downloadId, Request req, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(downloadId, new UpdateDownloadBuilder
        {
            Title = req.Title,
            Summary = req.Summary,
            Description = req.Description,
            Visibility = req.Visibility
        }, ct);

        await result.Match(
            download => SendAsync(new DownloadResponse
            {
                Id = download.Id.Value,
                Title = download.Title,
                Summary = download.Summary,
                Description = download.Description,
                Verification = download.Verification,
                Visibility = download.Visibility,
                UpdatedAt = download.UpdatedAt
            }, cancellation: ct),
            notFound => SendNotFoundAsync(ct));
    }
}