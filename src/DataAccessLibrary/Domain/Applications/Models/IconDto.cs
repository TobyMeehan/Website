namespace TobyMeehan.Com.Data.Domain.Applications.Models;

public class IconDto
{
    public required string ApplicationId { get; set; }
    public required string Filename { get; set; }
    public required Guid ObjectName { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
}