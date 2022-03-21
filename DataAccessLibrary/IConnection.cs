namespace TobyMeehan.Com.Data;

public interface IConnection
{
    Id<IConnection> Id { get; }
    Id<IApplication> AppId { get; }
    Id<IUser> UserId { get; }
    
    IUser User { get; }
    IApplication Application { get; }
}