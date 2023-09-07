namespace TobyMeehan.Com.Accounts.Authentication;

public interface IAuthenticationService
{
    Task SignInAsync(IUser user);

    Task SignOutAsync();
}