namespace TobyMeehan.Com.Accounts.Models;

public class AuthorizeRequestModel
{
    public string ResponseType { get; set; }
    public Id<IApplication> ClientId { get; set; }
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public Id<IRedirect> RedirectId { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
}