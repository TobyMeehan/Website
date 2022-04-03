using System;
using Slapper;

namespace TobyMeehan.Com.Data.SqlKata;

public class Application : IApplication
{
    public Id<IApplication> Id { get; set; }
    
    public Id<IUser> UserId { get; set; }
    public IUser Author { get; set; }

    public Id<IDownload> DownloadId { get; set; }
    public IDownload Download { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string RedirectUri { get; set; }
    public string Secret { get; set; }
}