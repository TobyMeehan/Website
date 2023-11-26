using TobyMeehan.Com.Api.Features.Users;

namespace TobyMeehan.Com.Api.Features.DownloadAuthors;

public class DownloadAuthorResponse
{
    public required UserResponse? User { get; set; }
    public Optional<bool> CanEdit { get; set; }
    public Optional<bool> CanManageAuthors { get; set; }
    public Optional<bool> CanManageFiles { get; set; }
    public Optional<bool> CanDelete { get; set; }
}