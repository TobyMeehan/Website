namespace TobyMeehan.Com.Api.Features.Transactions;

public class TransactionResponse
{
    public required Optional<string> Id { get; set; }
    public Optional<string> SenderId { get; set; }
    public required Optional<string> RecipientId { get; set; }
    public required Optional<bool> IsTransfer { get; set; }
    public required Optional<string> ApplicationId { get; set; }
    public required Optional<string?> Description { get; set; }
    public required Optional<double> Amount { get; set; }
    public required Optional<DateTime> SentAt { get; set; }
}
