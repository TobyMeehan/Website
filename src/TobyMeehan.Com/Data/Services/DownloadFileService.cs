using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Services;

public class DownloadFileService : IDownloadFileService
{
    private readonly IDownloadFileRepository _downloadFileRepository;
    private readonly IStorageService _storageService;

    public DownloadFileService(
        IDownloadFileRepository downloadFileRepository,
        IStorageService storageService)
    {
        _downloadFileRepository = downloadFileRepository;
        _storageService = storageService;
    }

    public async Task<(DownloadFile File, string UploadUrl)> CreateAsync(CreateDownloadFile create,
        CancellationToken cancellationToken = default)
    {
        var existingFile = await _downloadFileRepository.GetByFilenameAsync(create.DownloadId, create.Filename, 
                cancellationToken);

        var filename = existingFile is null 
            ? create.Filename 
            : await GenerateFilenameAsync(create, cancellationToken);
        
        var file = new DownloadFileDto
        {
            DownloadId = create.DownloadId,
            Filename = filename,
            ContentType = create.ContentType,
            SizeInBytes = create.SizeInBytes,
            Visibility = Visibility.Public,
            Status = FileStatus.Reserved,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        file = await _downloadFileRepository.CreateAsync(file, cancellationToken);

        var url = await _storageService.SignSingleUploadAsync(file.DownloadId.ToString(), file.Id.ToString(),
            file.ContentType);

        return (new DownloadFile
        {
            Id = file.Id,
            DownloadId = file.DownloadId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            Visibility = file.Visibility,
            Status = file.Status,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
        }, url);
    }

    private async Task<string> GenerateFilenameAsync(CreateDownloadFile file, CancellationToken cancellationToken)
    {
        var (filename, extension) = (Path.GetFileNameWithoutExtension(file.Filename), Path.GetExtension(file.Filename));
        
        foreach (var filenameAttempt in Enumerable.Range(1, 99).Select(i => $"{filename}({i}){extension}"))
        {
            var existingFile = await _downloadFileRepository.GetByFilenameAsync(file.DownloadId, filenameAttempt, 
                cancellationToken);

            if (existingFile is null)
            {
                return filenameAttempt;
            }
        }

        return Path.GetRandomFileName();
    }

    public async Task<FileUpload?> CreateUploadAsync(DownloadFile file,
        CancellationToken cancellationToken = default)
    {
        var upload = await _storageService.CreateMultipartUploadAsync(
            file.DownloadId.ToString(), file.Id.ToString(), file.ContentType, cancellationToken);

        if (upload is null)
        {
            return null;
        }

        return new FileUpload
        {
            Id = upload.Id,
            Key = upload.Key,
            DownloadId = file.DownloadId,
            FileId = file.Id
        };
    }

    public async Task<string> SignUploadPartAsync(DownloadFile file, string uploadId, int partNumber,
        CancellationToken cancellationToken = default)
    {
        var url = await _storageService.SignUploadPartAsync(
            file.DownloadId.ToString(), file.Id.ToString(), uploadId, partNumber);

        return url;
    }

    public async Task<IReadOnlyList<FileUploadPart>> GetUploadPartsAsync(DownloadFile file, string uploadId, CancellationToken cancellationToken = default)
    {
        var parts = await _storageService.GetUploadPartsAsync(file.DownloadId.ToString(), file.Id.ToString(), uploadId,
            cancellationToken);

        return parts.Select(part => new FileUploadPart
        {
            PartNumber = part.PartNumber,
            SizeInBytes = part.SizeInBytes,
            ETag = part.ETag
        }).ToList();
    }

    public async Task CompleteUploadAsync(DownloadFile file, string uploadId, IEnumerable<FileUploadPart> parts,
        CancellationToken cancellationToken = default)
    {
        await _storageService.CompleteMultipartUploadAsync(
            file.DownloadId.ToString(), file.Id.ToString(), uploadId, parts.Select(x => new UploadPartDto
            {
                PartNumber = x.PartNumber,
                SizeInBytes = x.SizeInBytes,
                ETag = x.ETag
            }), cancellationToken);
    }

    public async Task DeleteUploadAsync(DownloadFile file, string uploadId,
        CancellationToken cancellationToken = default)
    {
        await _storageService.AbortMultipartUploadAsync(
            file.DownloadId.ToString(), file.Id.ToString(), uploadId, cancellationToken);

        await _downloadFileRepository.DeleteAsync(file.DownloadId, file.Id, cancellationToken);
    }

    public async Task<string> CreateDownloadAsync(DownloadFile file, Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var url = await _storageService.SignDownloadAsync(file.DownloadId.ToString(), file.Id.ToString());

        var download = new FileDownloadDto
        {
            FileId = file.Id,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _downloadFileRepository.CreateDownloadAsync(download, cancellationToken);
        
        return url;
    }

    public async Task<DownloadFile?> GetByIdAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await _downloadFileRepository.GetByIdAsync(downloadId, fileId, cancellationToken);

        if (file is null)
        {
            return null;
        }
        
        return new DownloadFile
        {
            Id = file.Id,
            DownloadId = file.DownloadId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            Visibility = file.Visibility,
            Status = file.Status,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
        };
    }

    public async Task<DownloadFile?> GetByFilenameAsync(Guid downloadId, string filename,
        CancellationToken cancellationToken = default)
    {
        var file = await _downloadFileRepository.GetByFilenameAsync(downloadId, filename, cancellationToken);

        if (file is null)
        {
            return null;
        }

        return new DownloadFile
        {
            Id = file.Id,
            DownloadId = file.DownloadId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            Visibility = file.Visibility,
            Status = file.Status,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
        };
    }

    public async Task<IReadOnlyList<DownloadFile>> GetByDownloadAsync(Guid downloadId,
        CancellationToken cancellationToken = default)
    {
        var files = await _downloadFileRepository.GetByDownloadAsync(downloadId, cancellationToken);

        return files.Select(file => new DownloadFile
        {
            Id = file.Id,
            DownloadId = file.DownloadId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            Visibility = file.Visibility,
            Status = file.Status,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
        }).ToList();
    }

    public async Task<DownloadFile?> UpdateAsync(Guid downloadId, Guid fileId, UpdateDownloadFile update,
        CancellationToken cancellationToken = default)
    {
        var file = await _downloadFileRepository.GetByIdAsync(downloadId, fileId, cancellationToken);

        if (file is null)
        {
            return null;
        }

        file.Filename = update.Filename;

        await _downloadFileRepository.UpdateAsync(file, cancellationToken);

        return new DownloadFile
        {
            Id = file.Id,
            DownloadId = file.DownloadId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            Visibility = file.Visibility,
            Status = file.Status,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
        };
    }

    public async Task DeleteAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken = default)
    {
        var storageResult = await _storageService.DeleteAsync(downloadId.ToString(), fileId.ToString(), cancellationToken);

        if (!storageResult)
        {
            return;
        }
        
        await _downloadFileRepository.DeleteAsync(downloadId, fileId, cancellationToken);
    }
}