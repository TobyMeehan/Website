namespace TobyMeehan.Com.Data.Entities;

public class Redirect : IRedirect
{
    public required Id<IRedirect> Id { get; init; }
    public required Id<IApplication> ApplicationId { get; init; }
    public required Uri Uri { get; init; }
}