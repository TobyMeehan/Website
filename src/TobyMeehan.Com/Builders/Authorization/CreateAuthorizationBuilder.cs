using TobyMeehan.Com.Models.Authorization;

namespace TobyMeehan.Com.Builders.Authorization;

public struct CreateAuthorizationBuilder : ICreateAuthorization
{
    public CreateAuthorizationBuilder WithId(Id<IAuthorization> value) => this with { Id = value };
    public Optional<Id<IAuthorization>> Id { get; set; }
    
    public CreateAuthorizationBuilder WithApplication(Id<IApplication> value) => this with { Application = value };
    public Id<IApplication> Application { get; set; }

    public CreateAuthorizationBuilder WithUser(Id<IUser> value) => this with { User = value };
    public Id<IUser> User { get; set; }

    public CreateAuthorizationBuilder WithStatus(string? value) => this with { Status = value };
    public string? Status { get; set; }

    public CreateAuthorizationBuilder WithType(string? value) => this with { Type = value };
    public string? Type { get; set; }

    public CreateAuthorizationBuilder WithScopes(IEnumerable<string> value) => this with { Scopes = value };
    public IEnumerable<string> Scopes { get; set; }

    public CreateAuthorizationBuilder WithCreatedAt(DateTime value) => this with { CreatedAt = value };
    public DateTime CreatedAt { get; set; }
}