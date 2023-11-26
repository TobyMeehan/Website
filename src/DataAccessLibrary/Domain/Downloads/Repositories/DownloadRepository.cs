using SqlKata;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Downloads.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Downloads.Repositories;

public class DownloadRepository : Repository<DownloadDto>, IDownloadRepository
{
    public DownloadRepository(ISqlDataAccess db) : base(db, "downloads")
    {
    }

    private const string Authors = "downloadauthors";
    private readonly Query _authors = new Query(Authors);

    private const string Users = "users";
    private readonly Query _users = new Query(Users)
        .OrderBy("DisplayName");
    
    protected override Query Query()
    {
        return base.Query()
            .OrderByDesc("UpdatedAt")
            .OrderBy("Title")
            .LeftJoin(_authors.As(Authors), j => j.On($"{Authors}.DownloadId", Column("Id")))
            .LeftJoin(_users.As(Users), j => j.On($"{Users}.Id", $"{Authors}.UserId"))
            
            .Select($"{Table}.{{Id, Title, Summary, Description, Verification, Visibility, UpdatedAt}}",
                $"{Authors}.UserId AS Authors_UserId",
                $"{Authors}.CanEdit AS Authors_CanEdit",
                $"{Authors}.CanManageAuthors AS Authors_CanManageAuthors",
                $"{Authors}.CanManageFiles AS Authors_CanManageFiles",
                $"{Authors}.CanDelete AS Authors_CanDelete",
                $"{Users}.Username AS Authors_Username",
                $"{Users}.DisplayName AS Authors_DisplayName");
    }

    public IAsyncEnumerable<DownloadDto> SelectPublicAsync(LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<DownloadDto>(Query(limit)
                .Where(Column("Visibility"), VisibilityNames.Public),
            cancellationToken: ct);
    }

    public IAsyncEnumerable<DownloadDto> SelectByAuthorAsync(string userId, LimitStrategy? limit, CancellationToken ct)
    {
        var authors = new Query("downloadauthors").Where("UserId", userId).Select("DownloadId");

        return Db.QueryAsync<DownloadDto>(Query(limit)
                .WhereIn(Column("Id"), authors),
            cancellationToken: ct);
    }
}