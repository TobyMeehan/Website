using TobyMeehan.Com.Models.Scope;

namespace TobyMeehan.Com.Builders.Scope;

public struct CreateScopeBuilder : ICreateScope
{
    public CreateScopeBuilder WithAlias(string value) => this with { Alias = value };
    public Optional<string> Alias { get; set; }
    
    public CreateScopeBuilder WithName(string value) => this with { Name = value };
    public string Name { get; set; }

    public CreateScopeBuilder WithDisplayName(string value) => this with { DisplayName = value };
    public string DisplayName { get; set; }

    public CreateScopeBuilder WithDescription(string value) => this with { Description = value };
    public string Description { get; set; }
}