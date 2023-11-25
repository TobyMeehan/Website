namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class Icon : IFile
{
    public required string Filename { get; init; }
    public required MediaType ContentType { get; init; }
    public required long Size { get; init; }
}