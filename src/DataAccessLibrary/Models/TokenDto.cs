namespace TobyMeehan.Com.Data.Models;

public class TokenDto
{
    public required string Id { get; set; }
    public required string AuthorizationId { get; set; }
    public required string? Payload { get; set; }
    public required string? ReferenceId { get; set; }
    public required string? Status { get; set; }
    public required string? Type { get; set; }
    public required string? Subject { get; set; }
    public required DateTime? RedeemedAt { get; set; }
    public required DateTime? ExpiresAt { get; set; }
    public required DateTime CreatedAt { get; set; }
}