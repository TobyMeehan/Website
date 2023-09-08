using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class ApplicationRepository : Repository<ApplicationData>, IApplicationRepository
{
    public ApplicationRepository(QueryFactory db) : base(db, "applications")
    {
    }

    private const string Redirects = "redirects";

    protected override Query Query()
    {
        var redirects = new Query(Redirects).OrderBy("Uri");

        return base.Query()
            .OrderBy("Name")
            .LeftJoin(redirects.As(Redirects), j => j.On($"{Redirects}.ApplicationId", $"{Table}.Id"))

            .Select($"{Table}.{{Id, AuthorId, DownloadId, Name, Description}}",
                $"{Redirects}.Id AS Redirects_Id", $"{Redirects}.ApplicationId AS Redirects_ApplicationId",
                $"{Redirects}.Uri AS Redirects_Uri");
    }

    public async Task<List<ApplicationData>> SelectByAuthorAsync(string userId, CancellationToken ct)
    {
        return await QueryAsync(query => query.Where($"{Table}.AuthorId", userId), cancellationToken: ct);
    }

    public async Task AddRedirectAsync(RedirectData redirect, CancellationToken ct)
    {
        await Db.Query(Redirects).InsertAsync(redirect, cancellationToken: ct);
    }

    public async Task RemoveRedirectAsync(string redirectId, CancellationToken ct)
    {
        await Db.Query(Redirects).Where("Id", redirectId).DeleteAsync(cancellationToken: ct);
    }
}