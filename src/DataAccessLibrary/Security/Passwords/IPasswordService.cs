namespace TobyMeehan.Com.Data.Security.Passwords;

public interface IPasswordService
{
    Task<byte[]> HashAsync(Password password);

    Task<PasswordHashResult> CheckAsync(Password password, byte[] protectedHash);
}