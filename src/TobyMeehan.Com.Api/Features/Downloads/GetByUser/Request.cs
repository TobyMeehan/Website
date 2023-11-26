using FastEndpoints;
using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Downloads.GetByUser;

public class Request : AuthenticatedRequest
{
    [BindFrom("private")]
    public bool IncludePrivate { get; set; } = false;

    [BindFrom("unlisted")]
    public bool IncludeUnlisted { get; set; } = false;

    [BindFrom("public")]
    public bool IncludePublic { get; set; } = false;
}