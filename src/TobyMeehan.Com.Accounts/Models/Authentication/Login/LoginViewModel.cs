namespace TobyMeehan.Com.Accounts.Models.Authentication.Login;

public class LoginViewModel
{
    public required string ReturnUrl { get; set; }
    public required LoginFormModel Form { get; set; }
}