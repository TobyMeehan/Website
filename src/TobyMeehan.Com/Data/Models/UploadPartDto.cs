namespace TobyMeehan.Com.Data.Models;

public class UploadPartDto
{
    public int PartNumber { get; set; }
    public long SizeInBytes { get; set; }
    public string ETag { get; set; } = null!;
}