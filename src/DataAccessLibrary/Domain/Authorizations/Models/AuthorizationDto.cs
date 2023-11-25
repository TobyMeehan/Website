namespace TobyMeehan.Com.Data.Domain.Authorizations.Models;

public class AuthorizationDto
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required string UserId { get; set; }
    public required string? Status { get; set; }
    public required string? Type { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Scopes { get; set; }
}