namespace TobyMeehan.Com;

/// <summary>
/// A role for an application.
/// </summary>
public interface IApplicationRole : IEntity<IApplicationRole>, IRole<IApplication>
{
    /// <summary>
    /// The application of the role.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
}