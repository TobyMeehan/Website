namespace TobyMeehan.Com.Models.Download;

public interface ICreateDownload
{
    string Title { get; }
    string Summary { get; }
    string? Description { get; }
    string Visibility { get; }
    Id<IUser> User { get; }
}