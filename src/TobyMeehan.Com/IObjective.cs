namespace TobyMeehan.Com;

/// <summary>
/// A scoreboard objective.
/// </summary>
public interface IObjective : IEntity<IObjective>
{
    /// <summary>
    /// ID of the application.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
    
    /// <summary>
    /// Name of the objective.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The scores in the objective.
    /// </summary>
    IEntityCollection<IScore> Scores { get; }
}