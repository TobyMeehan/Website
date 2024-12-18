namespace TobyMeehan.Com.Features.Comments.Put;

public class Request
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
}