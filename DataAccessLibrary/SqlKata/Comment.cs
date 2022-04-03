using System;
using Slapper;

namespace TobyMeehan.Com.Data.SqlKata;

public class Comment : IComment
{
    public Id<IComment> Id { get; set; }

    public Id<IUser> UserId { get; set; }
    public IUser User { get; set; }

    public string EntityId { get; set; }

    public string Content { get; set; }
    public DateTime Sent { get; set; }
    public DateTime? Edited { get; set; }
}