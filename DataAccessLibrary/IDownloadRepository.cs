using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IDownloadRepository
{
    Task<IReadOnlyList<IDownload>> GetAsync();

    Task<IReadOnlyList<IDownload>> GetByAuthorAsync(Id<IUser> userId);

    Task<IDownload> GetByIdAsync(Id<IDownload> id);

    Task<IDownload> AddAsync(Action<NewDownload> download);

    Task<IDownload> UpdateAsync(Id<IDownload> id, Action<EditDownload> download);

    Task AddAuthorAsync(Id<IDownload> id, Id<IUser> userId);

    Task RemoveAuthorAsync(Id<IDownload> id, Id<IUser> userId);

    Task DeleteAsync(Id<IDownload> id);
}

public class EditDownload
{
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public Version Version { get; set; }
    public DownloadVisibility Visibility { get; set; }
    public DownloadVerification Verified { get; set; }
}

public class NewDownload
{
    public Id<IUser> UserId { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public Version Version { get; set; }
}