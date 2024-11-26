using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Downloads;

public class DownloadMapper : ResponseMapper<DownloadResponse, Download>
{
    public override DownloadResponse FromEntity(Download download)
    {
        return new DownloadResponse
        {
            Id = download.Url,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility switch
            {
                Visibility.Private => "private",
                Visibility.Public => "public",
                Visibility.Unlisted => "unlisted",
                _ => throw new InvalidOperationException(),
            },
            Verification = download.Verification switch
            {
                Verification.Verified => "verified",
                Verification.Dangerous => "dangerous",
                _ => "none"
            },
            Version = download.Version?.ToString(),
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt,
        };
    }
}