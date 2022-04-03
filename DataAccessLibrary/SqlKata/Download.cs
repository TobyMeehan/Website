using System;
using System.Collections.Generic;
using Slapper;

namespace TobyMeehan.Com.Data.SqlKata;

public class Download : IDownload
{
    public Id<IDownload> Id { get; set; }
    
    public IReadOnlyList<IDownloadFile> Files { get; set; }
    public IReadOnlyList<IUser> Authors { get; set; }
    
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public Version Version { get; set; }
    public DateTime? Updated { get; set; }
    public DownloadVisibility Visibility { get; set; }
    public DownloadVerification Verified { get; set; }
}