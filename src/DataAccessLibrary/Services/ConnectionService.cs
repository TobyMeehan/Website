using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Exceptions;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class ConnectionService : BaseService<IConnection, ConnectionData>, IConnectionService
{
    private readonly IConnectionRepository _db;
    private readonly IIdService _id;
    private readonly IApplicationService _applications;
    private readonly IUserService _users;

    public ConnectionService(IConnectionRepository db, IIdService id, IApplicationService applications, IUserService users) : base(db)
    {
        _db = db;
        _id = id;
        _applications = applications;
        _users = users;
    }

    protected override async Task<IConnection> MapperAsync(ConnectionData data)
    {
        var applicationId = new Id<IApplication>(data.ApplicationId);
        
        var application = await _applications.GetByIdAsync(applicationId);

        if (application is null)
        {
            throw new EntityNotFoundException<IApplication>(applicationId);
        }

        var userId = new Id<IUser>(data.UserId);
        
        var user = await _users.GetByIdAsync(userId);

        if (user is null)
        {
            throw new EntityNotFoundException<IUser>(userId);
        }
        
        return new Connection(data.Id, application, user, data.AutoAuthorize);
    }

    public async Task<IEntityCollection<IConnection>> GetByUserAsync(Id<IUser> user, CancellationToken ct)
    {
        var data = await _db.SelectByUserAsync(user.Value, ct);

        return await MapAsync(data);
    }

    public async Task<IConnection> GetOrCreateAsync(Id<IUser> user, Id<IApplication> application, bool autoAuthorize, CancellationToken ct)
    {
        var data = await _db.SelectByUserAndApplicationAsync(user.Value, application.Value, ct);

        if (data is null)
        {
            var id = await _id.GenerateAsync<IConnection>();

            data = new ConnectionData
            {
                Id = id.Value,
                ApplicationId = application.Value,
                UserId = user.Value,
                AutoAuthorize = autoAuthorize
            };

            await _db.InsertAsync(data, ct);
        }

        return (await MapAsync(data))! /* Required as NotNull attributes do not work with tasks */ ;
    }
}