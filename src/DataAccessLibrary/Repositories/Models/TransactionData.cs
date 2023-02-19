namespace TobyMeehan.Com.Data.Repositories.Models;

public class TransactionData
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string ApplicationId { get; set; }
    public string Description { get; set; }
    public double Amount { get; set; }
    public DateTimeOffset SentAt { get; set; }
}