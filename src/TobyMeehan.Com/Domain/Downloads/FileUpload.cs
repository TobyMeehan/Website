namespace TobyMeehan.Com.Domain.Downloads;

public class FileUpload
{
    public required string Id { get; init; }
    public required Guid DownloadId { get; init; }
    public required Guid FileId { get; init; }
    public required string Key { get; init; }
}