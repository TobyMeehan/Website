namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class Application : IApplication
{
    public required Id<IApplication> Id { get; init; }
    public required Id<IUser> AuthorId { get; init; }
    public Id<IDownload>? DownloadId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool HasSecret { get; init; }
    public IFile? Icon { get; init; }
    public required IEntityCollection<IRedirect> Redirects { get; init; }
}