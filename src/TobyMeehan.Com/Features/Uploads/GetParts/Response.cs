namespace TobyMeehan.Com.Features.Uploads.GetParts;

public class Response
{
    public required int PartNumber { get; set; }
    public required long Size { get; set; }
    public required string ETag { get; set; }
}