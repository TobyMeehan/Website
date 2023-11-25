using SqlKata;

namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class ApplicationDto
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }
    public string? DownloadId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public byte[]? Secret { get; set; }
    
    [Ignore]
    public IconDto? Icon { get; set; }
    
    [Ignore]
    public IReadOnlyList<RedirectDto> Redirects { get; set; } = new List<RedirectDto>();
}