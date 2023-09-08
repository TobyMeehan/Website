namespace TobyMeehan.Com.Data.Entities;

public class Application : Entity<IApplication>, IApplication
{
    public Application(string id, string authorId, string? downloadId, string name, string? description, IFile? icon, IEntityCollection<IRedirect> redirects) : base(id)
    {
        AuthorId = new Id<IUser>(authorId);
        DownloadId = downloadId is not null ? new Id<IDownload>(downloadId) : null;
        Name = name;
        Description = description;
        Icon = icon;
        Redirects = redirects;
    }

    public Id<IUser> AuthorId { get; }
    public Id<IDownload>? DownloadId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IFile? Icon { get; }
    public IEntityCollection<IRedirect> Redirects { get; }
}