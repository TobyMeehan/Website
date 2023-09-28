namespace TobyMeehan.Com.Builders;

/// <summary>
/// Builder structure used to create a new application session.
/// </summary>
public struct CreateSessionBuilder
{
    /// <summary>
    /// Sets the <see cref="Connection"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateSessionBuilder WithConnection(Id<IConnection> value) => this with { Connection = value };
    
    /// <summary>
    /// The connection to be associated with the session.
    /// </summary>
    public Id<IConnection> Connection { get; set; }

    /// <summary>
    /// Sets the <see cref="Redirect"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateSessionBuilder WithRedirect(Id<IRedirect> value) => this with { Redirect = value };
    
    /// <summary>
    /// The redirect to be used for the session.
    /// </summary>
    public Id<IRedirect> Redirect { get; set; }

    /// <summary>
    /// Sets the <see cref="Scope"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateSessionBuilder WithScope(string? value) => this with { Scope = value };
    
    /// <summary>
    /// The scope of the session.
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// Sets the <see cref="CanRefresh"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateSessionBuilder WithCanRefresh(bool value) => this with { CanRefresh = value };
    
    /// <summary>
    /// Whether the session can be refreshed using a refresh token.
    /// </summary>
    public bool CanRefresh { get; set; }
}