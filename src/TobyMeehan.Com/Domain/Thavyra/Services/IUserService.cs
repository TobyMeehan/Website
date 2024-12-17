namespace TobyMeehan.Com.Domain.Thavyra.Services;

public interface IUserService
{
    Task<User?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}