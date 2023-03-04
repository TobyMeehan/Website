namespace TobyMeehan.Com.Accounts.Services;

public interface IAuthenticationService
{
    Task SignInAsync(IUser user);

    Task SignOutAsync();
}