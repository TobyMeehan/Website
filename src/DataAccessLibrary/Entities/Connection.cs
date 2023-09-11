namespace TobyMeehan.Com.Data.Entities;

public class Connection : Entity<IConnection>, IConnection
{
    public Connection(string id, IApplication application, IUser user, bool autoAuthorize) : base(id)
    {
        Application = application;
        User = user;
        AutoAuthorize = autoAuthorize;
    }

    public IApplication Application { get; }
    public IUser User { get; }
    public bool AutoAuthorize { get; }
}