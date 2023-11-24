using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Transaction;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Transactions.Transfer;

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
        Post("/users/{UserId}/transactions/{RecipientId}");
        Policies(PolicyNames.Transaction.Scope.Transfer);
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
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, new Resource<ITransaction>(user),
            PolicyNames.Transaction.Operation.Transfer);

        if (authorizationResult.Succeeded)
        {
            await TransferAsync(user, application, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task TransferAsync(IUser user, Id<IApplication> application, Request req, CancellationToken ct)
    {
        var result = await _service.CreateTransferAsync(new CreateTransferBuilder
        {
            Application = application,
            Sender = user.Id,
            Recipient = new Id<IUser>(req.RecipientId),
            Description = req.Description | null as string,
            Amount = req.Amount
        }, ct);

        await result.Match(
            transaction => SendAsync(new TransactionResponse
            {
                Id = transaction.Id.Value,
                RecipientId = transaction.RecipientId.Value,
                SenderId = transaction.SenderId?.Value!,
                ApplicationId = transaction.ApplicationId.Value,
                IsTransfer = true,
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