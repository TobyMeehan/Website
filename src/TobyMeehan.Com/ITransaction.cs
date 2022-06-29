namespace TobyMeehan.Com;

/// <summary>
/// A transaction made to a user.
/// </summary>
public interface ITransaction : IEntity<ITransaction>
{
    /// <summary>
    /// The user.
    /// </summary>
    Id<IUser> UserId { get; }
    
    /// <summary>
    /// The application sending the transaction.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
    
    /// <summary>
    /// A description of the transaction.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// The amount of the transaction.
    /// </summary>
    double Amount { get; }
    
    /// <summary>
    /// The datetime the transaction was sent.
    /// </summary>
    DateTimeOffset SentAt { get; }
}