namespace TobyMeehan.Com.Accounts.Models.OpenId;

public class OpenIdAuthorization
{
    public Id<IAuthorization> Id { get; set; }
    public string? ApplicationId { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public List<string> Scopes { get; set; } = new();
    public DateTimeOffset? CreatedAt { get; set; }
}