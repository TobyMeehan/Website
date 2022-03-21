using System;

namespace TobyMeehan.Com.Data;

public interface IComment
{
    Id<IComment> Id { get; }
    Id<IUser> UserId { get; }
    string EntityId { get; }
    
    IUser User { get; }
    
    string Content { get; }
    
    DateTime Sent { get; }
    DateTime? Edited { get; }
}