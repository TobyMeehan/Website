namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class Redirect : IRedirect
{
    public required Id<IRedirect> Id { get; init; }
    public required Id<IApplication> ApplicationId { get; init; }
    public required Uri Uri { get; init; }
}