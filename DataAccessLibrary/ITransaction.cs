using System;

namespace TobyMeehan.Com.Data;

public interface ITransaction
{
    Id<ITransaction> Id { get; }
    Id<IUser> UserId { get; }
    Id<IApplication> AppId { get; }
    
    IApplication Application { get; }
    
    string Description { get; }
    int Amount { get; }
    DateTime Sent { get; }
}