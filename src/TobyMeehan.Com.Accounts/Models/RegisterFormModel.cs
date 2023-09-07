using System.Text.RegularExpressions;
using FluentValidation;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Models;

public class RegisterFormModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    
    public class Validator : AbstractValidator<RegisterFormModel>
    {
        public Validator(IUserService users)
        {
            RuleFor(model => model.Username)
                .NotEmpty()
                .Length(1, 40)
                .Matches(new Regex(@"([a-zA-Z0-9_-]+)"))
                .MustAsync(users.IsHandleUniqueAsync);

            RuleFor(model => model.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

            RuleFor(model => model.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}