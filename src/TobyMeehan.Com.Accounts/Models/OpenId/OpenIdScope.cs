namespace TobyMeehan.Com.Accounts.Models.OpenId;

public class OpenIdScope
{
    public Id<IScope> Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}