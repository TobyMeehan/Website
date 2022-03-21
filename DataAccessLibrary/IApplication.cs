namespace TobyMeehan.Com.Data;

public interface IApplication
{
    Id<IApplication> Id { get; }
    Id<IUser> UserId { get; }
    Id<IDownload> DownloadId { get; }
    
    IUser Author { get; }
    IDownload Download { get; }
    
    string Name { get; }
    string Description { get; }
    
    string IconUrl { get; }
    
    string RedirectUri { get; }
    string Secret { get; }
}