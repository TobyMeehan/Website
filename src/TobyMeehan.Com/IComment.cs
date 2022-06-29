namespace TobyMeehan.Com;

/// <summary>
/// Base entity for a comment.
/// </summary>
/// <typeparam name="TComment">Derived type of comment.</typeparam>
public interface IComment<TComment> : IEntity<TComment> where TComment : IEntity<TComment>
{
    /// <summary>
    /// The author of the comment.
    /// </summary>
    Id<IUser> AuthorId { get; }

    /// <summary>
    /// The message content of the comment.
    /// </summary>
    string Content { get; }
    
    /// <summary>
    /// The datetime the comment was sent.
    /// </summary>
    DateTimeOffset SentAt { get; }
    
    /// <summary>
    /// The datetime (if any) the comment was edited.
    /// </summary>
    DateTimeOffset? EditedAt { get; }
}