using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class UserService : BaseService<IUser, UserData, CreateUserBuilder>, IUserService
{
    private readonly IUserRepository _db;
    private readonly IIdService _id;
    private readonly IPasswordService _password;

    public UserService(IUserRepository db, IIdService id, IPasswordService password) : base(db)
    {
        _db = db;
        _id = id;
        _password = password;
    }
    
    protected override Task<IUser> MapAsync(UserData data)
    {
        return Task.FromResult<IUser>(new User(data.Id, data.Name, data.Handle, data.Balance, data.Description));
    }

    protected override async Task<UserData> CreateAsync(CreateUserBuilder create)
    {
        var id = await _id.GenerateAsync<IUser>();

        return new UserData
        {
            Id = id.Value,
            Handle = create.Username,
            Name = create.Username,
            HashedPassword = await _password.HashAsync(create.Password)
        };
    }

    public async Task<IUser?> FindByHandleAsync(string handle, CancellationToken ct)
    {
        var data = await _db.SelectByHandleAsync(handle, ct);

        if (data is null)
        {
            return null;
        }
        
        return await MapAsync(data);
    }

    public async Task<IUser?> FindByCredentialsAsync(string handle, Password password, CancellationToken ct)
    {
        var user = await _db.SelectByHandleAsync(handle, ct);

        if (user is null)
        {
            return null;
        }

        if (!await _password.CheckAsync(password, user.HashedPassword))
        {
            return null;
        }

        return await MapAsync(user);
    }

    public async Task<IEntityCollection<IUser>> GetByRoleAsync(Id<IUserRole> role, CancellationToken ct)
    {
        var data = await _db.SelectByRoleAsync(role.Value, ct);

        return await MapAsync(data);
    }

    public async Task<bool> IsHandleUniqueAsync(string handle, CancellationToken ct)
    {
        var user = await _db.SelectByHandleAsync(handle, ct);

        return user is null;
    }

    public async Task<IUser> UpdateAsync(Id<IUser> id, UpdateUserBuilder user, CancellationToken ct)
    {
        return await UpdateAsync(id, data =>
        {
            data.Description = user.Description | data.Description;
            data.Name = user.Name | data.Name;
        }, ct);
    }

    public async Task<IUser> ProtectedUpdateAsync(Id<IUser> id, ProtectedUpdateUserBuilder user, CancellationToken ct)
    {
        var hash = user.Password.IsChanged 
            ? await _password.HashAsync(user.Password.Value) 
            : new Optional<string>();

        return await UpdateAsync(id, data =>
        {
            data.Handle = user.Handle | data.Handle;
            data.HashedPassword = hash | data.HashedPassword;
        }, ct);
    }

    public async Task UpdateBalanceAsync(Id<IUser> id, double amount, CancellationToken ct)
    {
        await UpdateAsync(id, user =>
        {
            user.Balance += amount;
        }, ct);
    }
}