namespace TobyMeehan.Com.Features.Downloads;

public class DownloadResponse
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public required string Description { get; set; }
    public required string Visibility { get; set; }
    public required string Verification { get; set; }
    public required string? Version { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}