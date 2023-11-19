namespace TobyMeehan.Com.Accounts.Models.Authentication.Register;

public class RegisterViewModel
{
    public required string ReturnUrl { get; set; }
    public required RegisterFormModel Form { get; set; }
}