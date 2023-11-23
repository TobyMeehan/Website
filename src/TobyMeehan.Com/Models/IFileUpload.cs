namespace TobyMeehan.Com.Models;

public interface IFileUpload
{
    string? Filename { get; }
    MediaType ContentType { get; }
    long Size { get; }
    Stream Stream { get; }
}