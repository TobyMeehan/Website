using System.Text.RegularExpressions;
using FluentValidation;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Users.Update;

public class Validator : AbstractValidator<Request>
{
    public Validator(IUserService users)
    {
        When(x => x.Username.HasValue, () =>
        {
            RuleFor(user => user.Username.Value)
                .NotEmpty()
                .Length(1, 40)
                .Matches(new Regex(@"([a-zA-Z0-9_-]+)")).WithMessage("Username must only use letters, numbers, underscores _ , or dashes - .")
                .MustAsync(users.IsUsernameUniqueAsync).WithMessage("Username already taken.");
        });

        When(x => x.DisplayName.HasValue, () =>
        {
            RuleFor(user => user.DisplayName.Value)
                .NotEmpty()
                .Length(1, 40);
        });

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(user => user.Description.Value)
                .Length(1, 400);
        });
    }
}