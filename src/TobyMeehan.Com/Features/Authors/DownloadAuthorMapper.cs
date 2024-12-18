using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Authors;

public class DownloadAuthorMapper : ResponseMapper<DownloadAuthorResponse, DownloadAuthor>
{
    public override DownloadAuthorResponse FromEntity(DownloadAuthor e)
    {
        return new DownloadAuthorResponse
        {
            Id = e.UserId,
            IsOwner = e.IsOwner,
            CreatedAt = e.CreatedAt
        };
    }
}