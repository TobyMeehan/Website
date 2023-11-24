using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Transaction;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Transactions.Create;

public class Endpoint : Endpoint<Request, TransactionResponse>
{
    private readonly ITransactionService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(ITransactionService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Post("/users/{UserId}/transactions");
        Policies(PolicyNames.Transaction.Scope.Create);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId) || !req.TryGetApplicationId(out var applicationId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, applicationId, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Id<IApplication> application, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, new Resource<ITransaction>(user), PolicyNames.Transaction.Operation.Create);

        if (authorizationResult.Succeeded)
        {
            await CreateAsync(user, application, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task CreateAsync(IUser user, Id<IApplication> application, Request req, CancellationToken ct)
    {
        var result = await _service.CreateAsync(new CreateTransactionBuilder
        {
            Application = application,
            User = user.Id,
            Description = req.Description | null as string,
            Amount = req.Amount
        }, ct);

        await result.Match(
            transaction => SendAsync(new TransactionResponse
            {
                Id = transaction.Id.Value,
                RecipientId = transaction.RecipientId.Value,
                IsTransfer = false,
                ApplicationId = transaction.ApplicationId.Value,
                Description = transaction.Description,
                Amount = transaction.Amount,
                SentAt = transaction.SentAt
            }, cancellation: ct),
            insufficientBalance =>
            {
                AddError(r => r.Amount, "User does not have sufficient balance for the specified amount.");
                ThrowIfAnyErrors();
                return Task.CompletedTask;
            },
            notFound => SendNotFoundAsync(ct));
    }
}