namespace TobyMeehan.Com.Builders.Scope;

public struct CreateScopeBuilder
{
    public CreateScopeBuilder WithName(string value) => this with { Name = value };
    public string Name { get; set; }

    public CreateScopeBuilder WithDisplayName(string value) => this with { DisplayName = value };
    public string DisplayName { get; set; }

    public CreateScopeBuilder WithDescription(string value) => this with { Description = value };
    public string Description { get; set; }
}