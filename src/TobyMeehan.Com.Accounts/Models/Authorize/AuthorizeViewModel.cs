namespace TobyMeehan.Com.Accounts.Models.Authorize;

public class AuthorizeViewModel
{
    public required ApplicationViewModel Client { get; set; }
    public required IUser Owner { get; set; }
    public required IEnumerable<string> Scopes { get; set; }
    public required string ReturnUrl { get; set; }
}