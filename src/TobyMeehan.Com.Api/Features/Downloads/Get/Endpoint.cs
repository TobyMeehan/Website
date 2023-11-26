using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.Get;

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
        Get("/downloads/{DownloadId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IDownload>(req.DownloadId), cancellationToken: ct);

        await result.Match(
            download => AuthorizeAsync(download, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IDownload download, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, PolicyNames.Download.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(download, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IDownload download, CancellationToken ct)
    {
        await SendAsync(new DownloadResponse
        {
            Id = download.Id.Value,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Verification = download.Verification,
            Visibility = download.Visibility,
            UpdatedAt = download.UpdatedAt
        }, cancellation: ct);
    }
}