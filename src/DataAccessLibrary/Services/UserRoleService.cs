using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models.Role;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class UserRoleService : BaseService<IUserRole, RoleDto>, IUserRoleService
{
    private readonly IUserRoleRepository _db;
    private readonly IIdService _id;

    public UserRoleService(IUserRoleRepository db, IIdService id, ICacheService<RoleDto, Id<IUserRole>> cache) : base(cache)
    {
        _db = db;
        _id = id;
    }

    protected override async Task<IUserRole> MapAsync(RoleDto data)
    {
        return new UserRole
        {
            Id = new Id<IUserRole>(data.Id),
            Name = data.Name
        };
    }

    public IAsyncEnumerable<IUserRole> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IUserRole, NotFound>> GetByIdAsync(Id<IUserRole> id, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync<UserRole>(data);
    }

    public async Task<IUserRole> CreateAsync(ICreateRole role, CancellationToken cancellationToken = default)
    {
        var id = await _id.GenerateAsync<IUserRole>();

        var data = new RoleDto
        {
            Id = id.Value,
            Name = role.Name
        };

        await _db.InsertAsync(data, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IUserRole> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);

        return result > 0 ? new Success() : new NotFound();
    }
}