namespace TobyMeehan.Com.Features.Comments.PostReply;

public class Request
{
    public Guid CommentId { get; set; }
    public string Content { get; set; }
}