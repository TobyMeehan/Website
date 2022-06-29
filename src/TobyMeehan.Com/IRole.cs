namespace TobyMeehan.Com;

/// <summary>
/// Generic entity for a role.
/// </summary>
/// <typeparam name="TRole">Derived type of role.</typeparam>
public interface IRole<TRole> : IEntity<TRole> where TRole : IRole<TRole>
{
    /// <summary>
    /// The name of the role.
    /// </summary>
    string Name { get; }
}