using Microsoft.AspNetCore.Mvc;

namespace TobyMeehan.Com.Accounts.Models;

public class AuthorizeRequestModel
{
    [FromQuery(Name = OAuth.Parameters.ResponseType)]
    public string ResponseType { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.ClientId)]
    public string ClientId { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.CodeChallenge)]
    public string? CodeChallenge { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.CodeChallengeMethod)]
    public string? CodeChallengeMethod { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.RedirectUri)]
    public string? RedirectUri { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.Scope)]
    public string? Scope { get; set; }
    
    [FromQuery(Name = OAuth.Parameters.State)]
    public string? State { get; set; }
}