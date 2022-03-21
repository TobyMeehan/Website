using System;
using System.Collections.Generic;

namespace TobyMeehan.Com.Data
{
    public interface IDownload
    {
        Id<IDownload> Id { get; }
        
        string Title { get; }
        string ShortDescription { get; }
        string LongDescription { get; }

        Version Version { get; }
        DateTime? Updated { get; }
        
        DownloadVisibility Visibility { get; }
        DownloadVerification Verified { get; }
        
        IReadOnlyList<IDownloadFile> Files { get; }
        IReadOnlyList<IUser> Authors { get; }
    }
}