using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Exceptions;
using TobyMeehan.Com.Models.User;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class UserService : BaseService<IUser, UserDto>, IUserService
{
    private readonly IUserRepository _db;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public UserService(
        IUserRepository db, 
        IIdService id, 
        IPasswordService password, 
        ICacheService<UserDto, Id<IUser>> cache) : base(cache)
    {
        _db = db;
        _id = id;
        _password = password;
    }
    
    protected override Task<IUser> MapAsync(UserDto dto)
    {
        return Task.FromResult<IUser>(new User
        {
            Id = new Id<IUser>(dto.Id),
            Handle = dto.Username,
            Name = dto.DisplayName,
            Description = dto.Description,
            Balance = dto.Balance,
        });
    }

    public async Task<IUser?> FindByIdAsync(string id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Id == id) ?? await _db.SelectByIdAsync(id, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<IUser?> FindByUsernameAsync(string username, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Username == username) ??
                   await _db.SelectByUsernameAsync(username, cancellationToken);

        return await GetAsync(data);
    }

    public async Task<IUser?> FindByCredentialsAsync(string username, Password password, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Username == username) ??
                   await _db.SelectByUsernameAsync(username, cancellationToken);

        if (data is null)
        {
            return null;
        }
        
        if (!await _password.CheckAsync(password, data.HashedPassword))
        {
            return null;
        }

        return await GetAsync(data);
    }

    public async Task<IUser> GetByIdAsync(Id<IUser> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<IUser>(id);
        }
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public IAsyncEnumerable<IUser> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IUser> GetByRoleAsync(Id<IUserRole> role, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _db.SelectByUsernameAsync(username, cancellationToken) is null;
    }

    public async Task<IUser> CreateAsync(ICreateUser user, CancellationToken cancellationToken = default)
    {
        var id = await _id.GenerateAsync<IUser>();

        var data = new UserDto
        {
            Id = id.Value,
            Username = user.Username,
            HashedPassword = await _password.HashAsync(user.Password),
            DisplayName = user.Username,
            Balance = 1000 // TODO: configuration option
        };

        await _db.InsertAsync(data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task<IUser> UpdateAsync(Id<IUser> id, IUpdateUser user, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<IUser>(id);
        }

        data.Username = user.Username | data.Username;
        
        if (user.Password.HasValue)
        {
            data.HashedPassword = await _password.HashAsync(user.Password.Value);
        }

        data.DisplayName = user.DisplayName | data.DisplayName;
        data.Description = user.Description | data.Description;
        // TODO: avatar

        await _db.UpdateAsync(id.Value, data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task UpdateBalanceAsync(Id<IUser> id, double amount, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<IUser>(id);
        }

        data.Balance += amount;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        Cache.Set(id, data);
    }

    public async Task DeleteAsync(Id<IUser> id, CancellationToken cancellationToken = default)
    {
        await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);
    }
}