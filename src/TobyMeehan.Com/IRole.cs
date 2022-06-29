namespace TobyMeehan.Com;

/// <summary>
/// Generic entity for a role.
/// </summary>
/// <typeparam name="TRole">Derived type of role.</typeparam>
public interface IRole<TRole> where TRole : IEntity<TRole>
{
    /// <summary>
    /// The name of the role.
    /// </summary>
    string Name { get; }
}