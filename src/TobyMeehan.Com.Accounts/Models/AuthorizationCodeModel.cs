namespace TobyMeehan.Com.Accounts.Models;

public class AuthorizationCodeModel
{
    public Id<IApplication> ClientId { get; set; }
    public Id<IUser> UserId { get; set; }
    public Id<IRedirect> RedirectId { get; set; }

    public bool RequireRedirect { get; set; }
    
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? Scope { get; set; }
}