namespace TobyMeehan.Com.Data.Security.BCrypt;

public class BCryptPasswordService : IPasswordService
{
    public Task<string> HashAsync(Password password)
    {
        return Task.FromResult(BCrypt.HashPassword(password.ToString(), BCrypt.GenerateSalt()));
    }

    public Task<bool> CheckAsync(Password password, string hash)
    {
        return Task.FromResult(BCrypt.CheckPassword(password.ToString(), hash));
    }
}