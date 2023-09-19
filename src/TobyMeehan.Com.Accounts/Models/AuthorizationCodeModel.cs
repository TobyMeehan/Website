namespace TobyMeehan.Com.Accounts.Models;

public class AuthorizationCodeModel
{
    public string ClientId { get; set; }
    public string UserId { get; set; }
    public string RedirectId { get; set; }

    public bool RequireRedirect { get; set; }
    
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? Scope { get; set; }
}