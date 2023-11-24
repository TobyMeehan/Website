using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Transactions.Transfer;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        When(x => x.Description.HasValue, () =>
        {
            RuleFor(x => x.Description.Value)
                .Length(2, 40);
        });
        
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.RecipientId)
            .NotEmpty()
            .NotEqual(x => x.UserId);
    }
}