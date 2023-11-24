using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Transactions.Get;

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
        Get("/user/{UserId}/transactions/{Id}");
        Policies(PolicyNames.Transaction.Scope.Read);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _service.GetByIdAndUserAsync(new Id<ITransaction>(req.Id), userId, cancellationToken: ct);

        await result.Match(
            transaction => AuthorizeAsync(transaction, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(ITransaction transaction, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, transaction, PolicyNames.Transaction.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(transaction, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(ITransaction transaction, CancellationToken ct)
    {
        await SendAsync(new TransactionResponse
        {
            Id = transaction.Id.Value,
            RecipientId = transaction.RecipientId.Value,
            SenderId = transaction.SenderId?.Value ?? Optional<string>.Empty(),
            IsTransfer = transaction.SenderId.HasValue,
            ApplicationId = transaction.ApplicationId.Value,
            Description = transaction.Description,
            Amount = transaction.Amount,
            SentAt = transaction.SentAt
        }, cancellation: ct);
    }
}