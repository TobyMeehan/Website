namespace TobyMeehan.Com.Builders;

public struct CreateSessionBuilder
{
    public CreateSessionBuilder WithConnection(Id<IConnection> value) => this with { Connection = value };
    public Id<IConnection> Connection { get; set; }

    public CreateSessionBuilder WithRedirect(Id<IRedirect> value) => this with { Redirect = value };
    public Id<IRedirect> Redirect { get; set; }

    public CreateSessionBuilder WithScope(string? value) => this with { Scope = value };
    public string? Scope { get; set; }

    public CreateSessionBuilder WithCanRefresh(bool value) => this with { CanRefresh = value };
    public bool CanRefresh { get; set; }
}