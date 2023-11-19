using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Models.Authentication.Register;

public class RegisterFormModel
{
    [Remote("CheckUsername", "Authentication", 
        ErrorMessage = "Username already taken.",
        HttpMethod = "post")]
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    
    public class Validator : AbstractValidator<RegisterFormModel>
    {
        public Validator(IUserService users)
        {
            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Please choose a username.")
                .Length(1, 40).WithMessage("Username must be shorter than 40 characters.")
                .Matches(new Regex(@"([a-zA-Z0-9_-]+)")).WithMessage("Username must only use letters, numbers, underscores _ , or dashes - .")
                .MustAsync(users.IsUsernameUniqueAsync).WithMessage("Username already taken.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Please choose a password.")
                .MinimumLength(8).WithMessage("Password must have at least 8 characters.")
                .MaximumLength(100).WithMessage("Password must be shorter than 100 characters.");

            RuleFor(model => model.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}