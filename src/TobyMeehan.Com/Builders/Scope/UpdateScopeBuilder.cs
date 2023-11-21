using TobyMeehan.Com.Models.Scope;

namespace TobyMeehan.Com.Builders.Scope;

public struct UpdateScopeBuilder : IUpdateScope
{
    public UpdateScopeBuilder WithDisplayName(string value) => this with { DisplayName = value };
    public Optional<string> DisplayName { get; set; }

    public UpdateScopeBuilder WithDescription(string value) => this with { Description = value };
    public Optional<string> Description { get; set; }
}