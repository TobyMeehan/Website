namespace TobyMeehan.Com.Data.Repositories.Models;

public class ApplicationData : IData
{
    public string Id { get; set; }
    public string AuthorId { get; set; }
    public string? DownloadId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<RedirectData> Redirects { get; set; }
}