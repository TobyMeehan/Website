using SqlKata;

namespace TobyMeehan.Com.Data.Repositories.Models;

public class ApplicationData
{
    public string Id { get; set; }
    public string AuthorId { get; set; }
    public string? DownloadId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? SecretHash { get; set; }
    
    [Ignore]
    public List<RedirectData> Redirects { get; set; } = new();
}