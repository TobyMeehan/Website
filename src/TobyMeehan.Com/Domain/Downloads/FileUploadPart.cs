namespace TobyMeehan.Com.Domain.Downloads;

public class FileUploadPart
{
    public required int PartNumber { get; init; }
    public required long SizeInBytes { get; init; }
    public required string ETag { get; init; }
}