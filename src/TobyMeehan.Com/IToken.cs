namespace TobyMeehan.Com;

public interface IToken : IEntity<IToken>
{
    IAuthorization? Authorization { get; }
    
    string? Payload { get; }
    string? ReferenceId { get; }
    string? Status { get; }
    string? Type { get; }
    string? Subject { get; }
    
    DateTime? RedemptionDate { get; }
    DateTime? ExpiresAt { get; }
    DateTime CreatedAt { get; }
    
}