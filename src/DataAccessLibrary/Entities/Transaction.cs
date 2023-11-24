namespace TobyMeehan.Com.Data.Entities;

public class Transaction : ITransaction
{
    public required Id<ITransaction> Id { get; init; }
    public required Id<IUser> RecipientId { get; init; }
    public required Id<IUser>? SenderId { get; init; }
    public required Id<IApplication> ApplicationId { get; init; }
    public required string? Description { get; init; }
    public required double Amount { get; init; }
    public required DateTime SentAt { get; init; }
}