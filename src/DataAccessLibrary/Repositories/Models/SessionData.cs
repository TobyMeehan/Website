namespace TobyMeehan.Com.Data.Repositories.Models;

public class SessionData
{
    public string Id { get; set; }
    public string ConnectionId { get; set; }
    public string RedirectId { get; set; }
    public string AuthorizationCode { get; set; }
    public string? Scope { get; set; }
    public string? CodeChallenge { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiry { get; set; }
}