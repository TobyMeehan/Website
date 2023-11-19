namespace TobyMeehan.Com.Models.Token;

public interface ICreateToken
{
    Optional<Id<IToken>> Id { get; }
    Id<IAuthorization> Authorization { get; }
    string? Payload { get; }
    string? ReferenceId { get; }
    string? Status { get; }
    string? Type { get; }
    string? Subject { get; }
    DateTime? RedeemedAt { get; }
    DateTime? ExpiresAt { get; }
    DateTime CreatedAt { get; }
}