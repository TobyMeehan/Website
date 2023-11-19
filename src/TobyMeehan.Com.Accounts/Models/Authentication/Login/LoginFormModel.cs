using FluentValidation;

namespace TobyMeehan.Com.Accounts.Models.Authentication.Login;

public class LoginFormModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    
    public class Validator : AbstractValidator<LoginFormModel>
    {
        public Validator()
        {
            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Please enter your username.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Please enter your password.");
        }
    }
}