namespace TobyMeehan.Com.Data.Entities;

public class Token : IToken
{
    public required Id<IToken> Id { get; init; }
    public required IAuthorization? Authorization { get; init; }
    public required string Payload { get; init; }
    public required string? ReferenceId { get; init; }
    public required string? Status { get; init; }
    public required string Type { get; init; }
    public required DateTime? RedemptionDate { get; init; }
    public required DateTime? ExpiresAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}