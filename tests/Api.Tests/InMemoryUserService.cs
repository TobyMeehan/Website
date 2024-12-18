using TobyMeehan.Com.Domain.Thavyra;
using TobyMeehan.Com.Domain.Thavyra.Services;

namespace Api.Tests;

public class InMemoryUserService : IUserService
{
    private readonly List<User> _users = [];

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public void Clear()
    {
        _users.Clear();
    }
    
    public async Task<User?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        return null;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _users.FirstOrDefault(x => x.Id == id);
    }
}