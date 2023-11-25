namespace Migrations.Entities;

public class Application
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }
    public string? DownloadId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public byte[]? Secret { get; set; }
}