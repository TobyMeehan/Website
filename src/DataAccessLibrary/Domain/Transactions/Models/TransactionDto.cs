namespace TobyMeehan.Com.Data.Domain.Transactions.Models;

public class TransactionDto
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public string? RecipientId { get; set; }
    public required string ApplicationId { get; set; }
    public required string? Description { get; set; }
    public required double Amount { get; set; }
    public required DateTime SentAt { get; set; }
}