namespace TobyMeehan.Com.Features.Authors;

public class DownloadAuthorResponse
{
    public required Guid Id { get; set; }
    public required bool IsOwner { get; set; }
    public required DateTime? CreatedAt { get; set; }
}