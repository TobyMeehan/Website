using TobyMeehan.Com.Models.Token;

namespace TobyMeehan.Com.Builders.Token;

public struct UpdateTokenBuilder : IUpdateToken
{
    public UpdateTokenBuilder WithPayload(string value) => this with { Payload = value };
    public Optional<string> Payload { get; set; }
    
    public UpdateTokenBuilder WithStatus(string? value) => this with { Status = value };
    public Optional<string?> Status { get; set; }

    public UpdateTokenBuilder WithRedeemedAt(DateTime? value) => this with { RedeemedAt = value };
    public Optional<DateTime?> RedeemedAt { get; set; }
    
    public UpdateTokenBuilder WithExpiresAt(DateTime? value) => this with { ExpiresAt = value };
    public Optional<DateTime?> ExpiresAt { get; set; }
}