using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.Delete;

public class Endpoint : Endpoint<Request>
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
        Delete("/downloads/{DownloadId}");
        Policies(PolicyNames.Download.Scope.Delete);
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
            await _authorizationService.AuthorizeAsync(User, download, PolicyNames.Download.Operation.Delete);

        if (authorizationResult.Succeeded)
        {
            await DeleteAsync(download, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task DeleteAsync(IDownload download, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(download.Id, ct);

        await result.Match(
            success => SendNoContentAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }
}