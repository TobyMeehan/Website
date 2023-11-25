namespace TobyMeehan.Com.Data.Domain.Avatars.Models;

public class AvatarDto
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required Guid ObjectName { get; set; }
    public required string Filename { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
}