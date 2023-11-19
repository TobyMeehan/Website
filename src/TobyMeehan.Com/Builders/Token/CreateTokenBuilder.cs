using TobyMeehan.Com.Models.Token;

namespace TobyMeehan.Com.Builders.Token;

public struct CreateTokenBuilder : ICreateToken
{

    public CreateTokenBuilder WithId(Id<IToken> value) => this with { Id = value };
    public Optional<Id<IToken>> Id { get; set; }
    
    public CreateTokenBuilder WithAuthorization(Id<IAuthorization> value) => this with { Authorization = value };
    public Id<IAuthorization> Authorization { get; set; }

    public CreateTokenBuilder WithPayload(string? value) => this with { Payload = value };
    public string? Payload { get; set; }

    public CreateTokenBuilder WithReferenceId(string? value) => this with { ReferenceId = value };
    public string? ReferenceId { get; set; }

    public CreateTokenBuilder WithStatus(string? value) => this with { Status = value };
    public string? Status { get; set; }

    public CreateTokenBuilder WithType(string? value) => this with { Type = value };
    public string? Type { get; set; }

    public CreateTokenBuilder WithSubject(string? value) => this with { Subject = value };
    public string? Subject { get; set; }

    public CreateTokenBuilder WithRedeemedAt(DateTime? value) => this with { RedeemedAt = value };
    public DateTime? RedeemedAt { get; set; }

    public CreateTokenBuilder WithExpiresAt(DateTime? value) => this with { ExpiresAt = value };
    public DateTime? ExpiresAt { get; set; }

    public CreateTokenBuilder WithCreatedAt(DateTime value) => this with { CreatedAt = value };
    public DateTime CreatedAt { get; set; }
}