using System;
using Slapper;

namespace TobyMeehan.Com.Data.SqlKata;

public class Connection : IConnection
{
    public Id<IConnection> Id { get; set; }
    
    public Id<IApplication> AppId { get; set; }
    public IApplication Application { get; set; }
    
    public Id<IUser> UserId { get; set; }
    public IUser User { get; set; }
    
}