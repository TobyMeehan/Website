namespace TobyMeehan.Com.Data.Security;

public interface IPasswordService
{
    Task<string> HashAsync(Password password);

    Task<bool> CheckAsync(Password password, string hash);
}