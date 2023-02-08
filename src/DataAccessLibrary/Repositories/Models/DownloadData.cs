namespace TobyMeehan.Com.Data.Repositories.Models;

public class DownloadData : IData
{
    public string Id { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public Visibility Visibility { get; set; }
    public Version Version { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}