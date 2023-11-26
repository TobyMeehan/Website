using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.DownloadAuthors.Create;

public class Request : AuthenticatedRequest
{
    public string DownloadId { get; set; } = default!;

    public Optional<bool> CanEdit { get; set; }
    public Optional<bool> CanManageAuthors { get; set; }
    public Optional<bool> CanManageFiles { get; set; }
    public Optional<bool> CanDelete { get; set; }
}