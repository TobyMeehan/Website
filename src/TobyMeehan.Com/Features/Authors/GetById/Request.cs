namespace TobyMeehan.Com.Features.Authors.GetById;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public Guid UserId { get; set; }
}