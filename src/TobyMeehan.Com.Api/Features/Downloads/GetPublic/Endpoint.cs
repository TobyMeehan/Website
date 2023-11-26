using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.GetPublic;

public class Endpoint : EndpointWithoutRequest<List<DownloadResponse>>
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
        Get("/downloads");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var downloads = await _service
            .GetPublicAsync(cancellationToken: ct)
            .ToListAsync(ct);

        await AuthorizeAsync(downloads, ct);
    }

    private async Task AuthorizeAsync(IEnumerable<IDownload> downloads, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync<IDownload>(User, downloads, PolicyNames.Download.Operation.Read);

        await GetAsync(authorizationResult.AuthorizedResources, ct);
    }

    private async Task GetAsync(IEnumerable<IDownload> downloads, CancellationToken ct)
    {
        await SendAsync(downloads.Select(download =>
            new DownloadResponse
            {
                Id = download.Id.Value,
                Title = download.Title,
                Summary = download.Summary,
                Description = download.Description,
                Verification = download.Verification,
                Visibility = download.Visibility,
                UpdatedAt = download.UpdatedAt
            }).ToList(), cancellation: ct);
    }
}