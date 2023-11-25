using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Applications.Models;

namespace TobyMeehan.Com.Data.Domain.Applications.Repositories;

public class IconRepository : Repository<IconDto>, IIconRepository
{
    public IconRepository(ISqlDataAccess db) : base(db, "applicationicons")
    {
    }

    public async Task<int> DeleteByApplicationAsync(string applicationId, CancellationToken cancellationToken)
    {
        return await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("ApplicationId"), applicationId),
            cancellationToken);
    }
}