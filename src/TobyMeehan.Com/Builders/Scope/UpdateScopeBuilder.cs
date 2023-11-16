namespace TobyMeehan.Com.Builders.Scope;

public struct UpdateScopeBuilder
{
    public UpdateScopeBuilder WithDisplayName(string value) => this with { DisplayName = value };
    public Optional<string> DisplayName { get; set; }

    public UpdateScopeBuilder WithDescription(string value) => this with { Description = value };
    public Optional<string> Description { get; set; }
}