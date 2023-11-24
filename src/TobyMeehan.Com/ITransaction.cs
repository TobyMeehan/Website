namespace TobyMeehan.Com;

/// <summary>
/// A transaction made to a user.
/// </summary>
public interface ITransaction : IEntity<ITransaction>
{
    /// <summary>
    /// The user that received the transaction.
    /// </summary>
    Id<IUser> RecipientId { get; }
    
    /// <summary>
    /// If the transaction is a transfer, the user that sent the transfer.
    /// </summary>
    Id<IUser>? SenderId { get; }
    
    /// <summary>
    /// The application sending the transaction.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
    
    /// <summary>
    /// A description of the transaction.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// The amount credited/debited by the transaction.
    /// </summary>
    double Amount { get; }
    
    /// <summary>
    /// When the transaction occurred.
    /// </summary>
    DateTime SentAt { get; }
}