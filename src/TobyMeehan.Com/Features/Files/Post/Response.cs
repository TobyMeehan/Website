namespace TobyMeehan.Com.Features.Files.Post;

public class Response : DownloadFileResponse
{
    public required string? UploadUrl { get; set; }
}