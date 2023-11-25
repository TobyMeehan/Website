using Microsoft.Extensions.Options;
using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Storage;
using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.User;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class UserService : BaseService<IUser, UserDto>, IUserService
{
    private readonly IUserRepository _db;
    private readonly IUserRoleService _userRoles;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public UserService(
        IUserRepository db, 
        IUserRoleService userRoles,
        IIdService id, 
        IPasswordService password,
        ICacheService<UserDto, Id<IUser>> cache) : base(cache)
    {
        _db = db;
        _userRoles = userRoles;
        _id = id;
        _password = password;
    }

    private static IUserRole MapRole(RoleDto role)
    {
        return new UserRole
        {
            Id = new Id<IUserRole>(role.Id),
            Name = role.Name
        };
    }
    
    protected override Task<IUser> MapAsync(UserDto dto)
    {
        var user = new User
        {
            Id = new Id<IUser>(dto.Id),
            Username = dto.Username,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Balance = dto.Balance,
            Roles = EntityCollection<IUserRole>.Create(dto.Roles, MapRole)
        };

        if (dto.Avatar is { } avatar)
        {
            user.Avatar = new Avatar
            {
                Id = new Id<IAvatar>(avatar.Id),
                Filename = avatar.Filename,
                ContentType = MediaType.Parse(avatar.ContentType),
                Size = avatar.Size
            };
        }
        
        return Task.FromResult<IUser>(user);
    }

    public async Task<OneOf<IUser, NotFound>> GetByUsernameAsync(string username, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Username == username) ??
                   await _db.SelectByUsernameAsync(username, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }
        
        return await GetAsync<User>(data);
    }

    public async Task<OneOf<IUser, InvalidCredentials, NotFound>> GetByCredentialsAsync(string username, Password password,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Username == username) ??
                   await _db.SelectByUsernameAsync(username, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }
        
        if (!await _password.CheckAsync(password, data.HashedPassword))
        {
            return new InvalidCredentials();
        }

        return await GetAsync<User>(data);
    }

    public async Task<OneOf<IUser, InvalidCredentials, NotFound>> GetByCredentialsAsync(Id<IUser> id, Password password, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (!await _password.CheckAsync(password, data.HashedPassword))
        {
            return new InvalidCredentials();
        }

        return await GetAsync<User>(data);
    }

    public async Task<OneOf<IUser, NotFound>> GetByIdAsync(Id<IUser> id, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync<User>(data);
    }

    public IAsyncEnumerable<IUser> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IUser> GetByRoleAsync(Id<IUserRole> role, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByRoleAsync(role.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
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

        return await GetAsync(data);
    }

    public async Task<OneOf<IUser, NotFound>> UpdateAsync(Id<IUser> id, IUpdateUser user,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (user.Username.HasValue && await IsUsernameUniqueAsync(user.Username.Value, cancellationToken))
        {
            data.Username = user.Username.Value;
        }
        
        if (user.Password.HasValue)
        {
            data.HashedPassword = await _password.HashAsync(user.Password.Value);
        }

        data.AvatarId = user.Avatar.MapOr(x => x?.Value, data.AvatarId);
        data.DisplayName = user.DisplayName | data.DisplayName;
        data.Description = user.Description | data.Description;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return await GetAsync<User>(data);
    }

    public async Task<OneOf<Success, InsufficientBalance, NotFound>> UpdateBalanceAsync(Id<IUser> id, double amount,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        if (data.Balance + amount < 0)
        {
            return new InsufficientBalance();
        }
        
        data.Balance += amount;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return new Success();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IUser> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task<OneOf<Success, NotFound>> AddRoleAsync(Id<IUser> id, Id<IUserRole> role, CancellationToken cancellationToken = default)
    {
        var roleResult = await _userRoles.GetByIdAsync(role, cancellationToken);

        if (roleResult.IsNotFound())
        {
            return new NotFound<IUserRole>();
        }

        var user = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (user is null)
        {
            return new NotFound<IUser>();
        }

        await _db.AddRoleAsync(id.Value, role.Value, cancellationToken);
        
        Cache.Remove(id);

        return new Success();
    }

    public async Task<OneOf<Success, NotFound>> RemoveRoleAsync(Id<IUser> id, Id<IUserRole> role, CancellationToken cancellationToken = default)
    {
        int result = await _db.RemoveRoleAsync(id.Value, role.Value, cancellationToken);

        return result > 0 ? new Success() : new NotFound();
    }
}