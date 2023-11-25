using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Applications.SetIcon;

public class Request : AuthenticatedRequest
{
    public IFormFile? Icon { get; set; }
}