namespace TobyMeehan.Com.Data.Models;

public class ReplyDto
{
    public Guid ParentId { get; set; }
    public Guid ReplyId { get; set; }

    public CommentDto? Parent { get; set; }
    public CommentDto? Reply { get; set; }
}