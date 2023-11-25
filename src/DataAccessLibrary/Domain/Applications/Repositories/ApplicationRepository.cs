using SqlKata;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Applications.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Applications.Repositories;

public class ApplicationRepository : Repository<ApplicationDto>, IApplicationRepository
{
    public ApplicationRepository(ISqlDataAccess db) : base(db, "applications")
    {
    }

    private const string Redirects = "redirects";

    private readonly Query _redirects = new Query(Redirects)
        .OrderBy("Uri");

    private const string Icons = "applicationicons";

    private readonly Query _icons = new Query(Icons);
    
    protected override Query Query()
    {
        return base.Query()
            .OrderBy("Name")
            .LeftJoin(_redirects.As(Redirects), j => j.On($"{Redirects}.ApplicationId", $"{Table}.Id"))
            .LeftJoin(_icons.As(Icons), j => j.On($"{Icons}.ApplicationId", $"{Table}.Id"))
            
            .Select($"{Table}.{{Id, AuthorId, DownloadId, Name, Description, Secret}}",
                $"{Redirects}.Id AS Redirects_Id", 
                $"{Redirects}.ApplicationId AS Redirects_ApplicationId",
                $"{Redirects}.Uri AS Redirects_Uri",
                
                $"{Icons}.Filename AS Icon_Filename",
                $"{Icons}.ObjectName AS Icon_ObjectName",
                $"{Icons}.ContentType AS Icon_ContentType",
                $"{Icons}.Size AS Icon_Size");
    }

    public IAsyncEnumerable<ApplicationDto> SelectByAuthorAsync(string userId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<ApplicationDto>(Query(limit)
                .Where(Column("AuthorId"), userId),
            cancellationToken: ct);
    }

    public IAsyncEnumerable<ApplicationDto> SelectByRedirectAsync(string uri, LimitStrategy? limit, CancellationToken ct)
    {
        var redirects = _redirects
            .Where("Uri", uri)
            .Select("ApplicationId");

        return Db.QueryAsync<ApplicationDto>(Query(limit)
                .WhereIn(Column("Id"), redirects),
            cancellationToken: ct);
    }

    public async Task AddRedirectAsync(RedirectDto redirect, CancellationToken ct)
    {
        await Db.ExecuteAsync(_redirects
                .AsInsert(redirect), 
            cancellationToken: ct);
    }

    public async Task RemoveRedirectAsync(string redirectId, CancellationToken ct)
    {
        await Db.ExecuteAsync(_redirects
                .AsDelete()
                .Where("Id", redirectId), 
            cancellationToken: ct);
    }
}