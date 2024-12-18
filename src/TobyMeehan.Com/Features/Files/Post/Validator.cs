using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Files.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Filename)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .NotEmpty();
    }
}