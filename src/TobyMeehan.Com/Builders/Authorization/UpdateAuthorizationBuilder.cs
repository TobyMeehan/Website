using TobyMeehan.Com.Models.Authorization;

namespace TobyMeehan.Com.Builders.Authorization;

public struct UpdateAuthorizationBuilder : IUpdateAuthorization
{
    public UpdateAuthorizationBuilder WithStatus(string? value) => this with { Status = value };
    public Optional<string?> Status { get; set; }

    public UpdateAuthorizationBuilder WithType(string? value) => this with { Type = value };
    public Optional<string?> Type { get; set; }

    public UpdateAuthorizationBuilder WithScopes(IEnumerable<string> value) => this with { Scopes = Optional<IEnumerable<string>>.Of(value) };
    public Optional<IEnumerable<string>> Scopes { get; set; }

    public UpdateAuthorizationBuilder WithCreatedAt(DateTime value) => this with { CreatedAt = value };
    public Optional<DateTime> CreatedAt { get; set; }
}