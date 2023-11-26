using SqlKata;

namespace TobyMeehan.Com.Data.Domain.Downloads.Models;

public class DownloadDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public required string? Description { get; set; }
    public required string Verification { get; set; }
    public required string Visibility { get; set; }
    public required DateTime UpdatedAt { get; set; }

    [Ignore] public IReadOnlyList<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
}