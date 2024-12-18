using System.Net;
using System.Net.Http.Headers;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Domain.Thavyra;
using TobyMeehan.Com.Domain.Thavyra.Services;

namespace TobyMeehan.Com.Data.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<User?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "api/users/@me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();
        
        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            Username = user.Username,
            ProfileUrl = _httpClient.BaseAddress + $"@{user.Username}",
            AvatarUrl = _httpClient.BaseAddress + $"api/users/{user.Id}/avatar.png"
        };
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + $"api/users/{id}");
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();
        
        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
        
        if (user is null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            Username = user.Username,
            ProfileUrl = _httpClient.BaseAddress + $"@{user.Username}",
            AvatarUrl = _httpClient.BaseAddress + $"api/users/{user.Id}/avatar.png"
        };
    }
}