using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IDownloadRepository
{
    Task<IReadOnlyList<IDownload>> GetAsync();

    Task<IReadOnlyList<IDownload>> GetByAuthorAsync(Id<IUser> userId);

    Task<IDownload> GetByIdAsync(string id);

    Task<IDownload> AddAsync(Action<NewDownload> download);

    Task<IDownload> UpdateAsync(Id<IDownload> id, Action<EditDownload> download);

    Task AddAuthorAsync(Id<IDownload> id, Id<IUser> userId);

    Task RemoveAuthorAsync(Id<IDownload> id, Id<IUser> userId);

    Task DeleteAsync(Id<IDownload> id);
}

public class EditDownload
{
    public string Title { get; set; } = null;
    public string ShortDescription { get; set; } = null;
    public string LongDescription { get; set; } = null;
    public Version Version { get; set; } = null;
    public DownloadVisibility? Visibility { get; set; } = null;
    public DownloadVerification? Verified { get; set; } = null;
}

public class NewDownload
{
    public Id<IUser>? UserId { get; set; } = null;
    public string Title { get; set; } = null;
    public string ShortDescription { get; set; } = null;
    public string LongDescription { get; set; } = null;
    public Version Version { get; set; } = null;
}