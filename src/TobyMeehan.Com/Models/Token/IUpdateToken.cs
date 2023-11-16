namespace TobyMeehan.Com.Models.Token;

public interface IUpdateToken
{
    Optional<string> Payload { get; }
    Optional<string?> Status { get; }
    Optional<DateTime?> RedeemedAt { get; }
    Optional<DateTime?> ExpiresAt { get; }
}