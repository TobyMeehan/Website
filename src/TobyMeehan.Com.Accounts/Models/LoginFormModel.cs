using FluentValidation;

namespace TobyMeehan.Com.Accounts.Models;

public class LoginFormModel
{
    public string Handle { get; set; }
    public string Password { get; set; }
    
    public class Validator : AbstractValidator<LoginFormModel>
    {
        public Validator()
        {
            RuleFor(model => model.Handle)
                .NotEmpty();

            RuleFor(model => model.Password)
                .NotEmpty();
        }
    }
}