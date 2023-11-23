using SqlKata;

namespace TobyMeehan.Com.Data.Models;

public class ApplicationDto
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }
    public string? DownloadId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? SecretHash { get; set; }
    
    [Ignore]
    public IReadOnlyList<RedirectDto> Redirects { get; set; } = new List<RedirectDto>();
}