namespace TobyMeehan.Com.Models.Token;

public interface ICreateToken
{
    Id<IAuthorization> Authorization { get; }
    string Payload { get; }
    string? ReferenceId { get; }
    string Status { get; }
    string Type { get; }
    DateTime? RedeemedAt { get; }
    DateTime ExpiresAt { get; }
    DateTime CreatedAt { get; }
}