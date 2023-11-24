using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Transactions.Create;

public class Request : AuthenticatedRequest
{
    public Optional<string?> Description { get; set; }
    public required double Amount { get; set; }
}