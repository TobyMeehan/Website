namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class RedirectDto
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required string Uri { get; set; }
}