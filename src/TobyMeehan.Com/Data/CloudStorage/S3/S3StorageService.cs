using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.CloudStorage.S3;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly StorageOptions _options;

    public S3StorageService(IAmazonS3 s3, IOptions<StorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<string> SignSingleUploadAsync(string prefix, string key, string contentType)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new GetPreSignedUrlRequest
        {
            Verb = HttpVerb.PUT,
            BucketName = _options.Bucket,
            Key = objectKey,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        var url = await _s3.GetPreSignedURLAsync(request);
        
        if (url is null)
        {
            throw new InvalidOperationException("Presigned URL request failed.");
        }

        return url;
    }

    public async Task<UploadDto?> CreateMultipartUploadAsync(string prefix, string key, string contentType, CancellationToken cancellationToken)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new InitiateMultipartUploadRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            ContentType = contentType
        };

        try
        {
            var response = await _s3.InitiateMultipartUploadAsync(request, cancellationToken);

            return response.HttpStatusCode switch
            {
                HttpStatusCode.OK => new UploadDto
                {
                    Id  = response.UploadId,
                    Key = objectKey,
                },
                _ => null
            };
        }
        catch (AmazonS3Exception)
        {
            return null;
        }
    }

    public async Task<string> SignUploadPartAsync(string prefix, string key, string uploadId, int partNumber)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new GetPreSignedUrlRequest
        {
            Verb = HttpVerb.PUT,
            BucketName = _options.Bucket,
            Key = objectKey,
            UploadId = uploadId,
            PartNumber = partNumber,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        
        var url = await _s3.GetPreSignedURLAsync(request);

        if (url is null)
        {
            throw new InvalidOperationException("Presigned URL request failed.");
        }

        return url;
    }

    public async Task<IReadOnlyList<UploadPartDto>> GetUploadPartsAsync(string prefix, string key, string uploadId, CancellationToken cancellationToken)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new ListPartsRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            UploadId = uploadId
        };
        
        var response = await _s3.ListPartsAsync(request, cancellationToken);

        return response.Parts.Select(part => new UploadPartDto
        {
            PartNumber = part.PartNumber,
            SizeInBytes = part.Size,
            ETag = part.ETag
        }).ToList();
    }

    public async Task AbortMultipartUploadAsync(string prefix, string key, string uploadId, CancellationToken cancellationToken)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new AbortMultipartUploadRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            UploadId = uploadId
        };

        await _s3.AbortMultipartUploadAsync(request, cancellationToken);
    }

    public async Task CompleteMultipartUploadAsync(string prefix, string key, string uploadId, IEnumerable<UploadPartDto> parts, CancellationToken cancellationToken)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new CompleteMultipartUploadRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            UploadId = uploadId,
            PartETags = parts.Select(part => new PartETag
            {
                PartNumber = part.PartNumber,
                ETag = part.ETag
            }).ToList()
        };
        
        await _s3.CompleteMultipartUploadAsync(request, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string prefix, string key, CancellationToken cancellationToken)
    {
        var objectKey = string.Join('/', prefix, key);

        var request = new DeleteObjectRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey
        };

        try
        {
            var response = await _s3.DeleteObjectAsync(request, cancellationToken);
            
            return response.HttpStatusCode == HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception)
        {
            return false;
        }
    }
}
