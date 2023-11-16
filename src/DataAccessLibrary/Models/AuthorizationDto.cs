namespace TobyMeehan.Com.Data.Models;

public class AuthorizationDto
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required string UserId { get; set; }
    public required string? Status { get; set; }
    public required string? Type { get; set; }
    public required DateTime CreatedAt { get; set; }
    public List<AuthorizationScopeDto> Scopes { get; set; } = new();
}

public class AuthorizationScopeDto
{
    public required string Id { get; set; }
}