using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Transactions.Transfer;

public class Request : AuthenticatedRequest
{
    public Optional<string?> Description { get; set; }
    public required double Amount { get; set; }
    public string RecipientId { get; set; } = default!;
}