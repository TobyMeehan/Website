namespace TobyMeehan.Com.Models.Download;

public interface IUpdateDownload
{
    Optional<string> Title { get; }
    Optional<string> Summary { get; }
    Optional<string?> Description { get; }
    Optional<string> Verification { get; }
    Optional<string> Visibility { get; }
}