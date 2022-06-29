namespace TobyMeehan.Com;

/// <summary>
/// A scoreboard score.
/// </summary>
public interface IScore : IEntity<IScore>
{
    /// <summary>
    /// The objective.
    /// </summary>
    Id<IObjective> ObjectiveId { get; }
    
    /// <summary>
    /// The score's user.
    /// </summary>
    Id<IUser> UserId { get; }
    
    /// <summary>
    /// The value of the score.
    /// </summary>
    double Value { get; }
}