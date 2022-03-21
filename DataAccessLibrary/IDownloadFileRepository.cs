using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Upload;

namespace TobyMeehan.Com.Data;

public interface IDownloadFileRepository
{
    Task<IReadOnlyList<IDownloadFile>> GetByDownloadAsync(Id<IDownload> downloadId);

    Task<IReadOnlyList<IDownloadFile>> GetByDownloadAndFilenameAsync(Id<IDownload> downloadId, string filename);

    Task<IDownloadFile> GetByIdAsync(string id);

    Task DownloadAsync(Id<IDownloadFile> id, Stream stream);

    Task<IDownloadFile> AddAsync(Action<NewDownloadFile> file, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null);

    Task<IDownloadFile> UpdateAsync(Id<IDownloadFile> id, Action<EditDownloadFile> file);

    Task DeleteAsync(Id<IDownloadFile> id);
}

public class EditDownloadFile
{
    public string Filename { get; set; } = null;
}

public class NewDownloadFile
{
    public Id<IDownload>? DownloadId { get; set; } = null;
    public string Filename { get; set; } = null;
    public Stream UploadStream { get; set; } = null;
}