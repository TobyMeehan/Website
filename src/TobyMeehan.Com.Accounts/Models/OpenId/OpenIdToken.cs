namespace TobyMeehan.Com.Accounts.Models.OpenId;

public class OpenIdToken
{
    public Id<IToken> Id { get; set; }
    public string? ApplicationId { get; set; }
    public string? AuthorizationId { get; set; }

    public string? ReferenceId { get; set; }
    public string? Payload { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Subject { get; set; }

    public DateTimeOffset? RedeemedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}