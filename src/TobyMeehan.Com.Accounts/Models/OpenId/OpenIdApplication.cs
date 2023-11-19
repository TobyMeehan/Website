namespace TobyMeehan.Com.Accounts.Models.OpenId;

public class OpenIdApplication
{
    public required Id<IApplication> Id { get; set; }
    public required string Name { get; set; }
    public required string ClientType { get; set; }
    public required IEntityCollection<IRedirect> Redirects { get; set; }
}