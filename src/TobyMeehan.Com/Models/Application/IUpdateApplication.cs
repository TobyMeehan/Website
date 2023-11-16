namespace TobyMeehan.Com.Models.Application;

public interface IUpdateApplication
{
    Optional<Id<IDownload>?> Download { get; }
    Optional<string> Name { get; }
    Optional<string?> Description { get; }
    Optional<IFileUpload> Icon { get; }
}