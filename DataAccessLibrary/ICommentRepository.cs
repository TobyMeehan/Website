using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface ICommentRepository
{
    Task<IComment> GetByIdAsync(Id<IComment> id);
    
    Task<IReadOnlyList<IComment>> GetByEntityAsync(string entityId);

    Task<IComment> AddAsync(Action<NewComment> comment);

    Task<IComment> UpdateAsync(Id<IComment> id, Action<EditComment> comment);

    Task DeleteAsync(Id<IComment> id);
}

public class NewComment
{
    public string EntityId { get; set; } = null;
    public Id<IUser>? UserId { get; set; } = null;
    public string Content { get; set; } = null;
}

public class EditComment
{
    public string Content { get; set; } = null;
}

