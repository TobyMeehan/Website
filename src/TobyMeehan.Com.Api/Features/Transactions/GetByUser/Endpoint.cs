using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Transactions.GetByUser;

public class Endpoint : Endpoint<AuthenticatedRequest, List<TransactionResponse>>
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
        Get("/users/{UserId}/transactions");
        Policies(PolicyNames.Transaction.Scope.Read);
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            async user =>
            {
                var transactions = await _service
                    .GetByUserAsync(user.Id, cancellationToken: ct)
                    .ToListAsync(ct);

                await AuthorizeAsync(transactions, user, ct);
            },
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IEnumerable<ITransaction> transactions, IUser user, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, transactions, user, PolicyNames.Transaction.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(authorizationResult.AuthorizedResources, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IEnumerable<ITransaction> transactions, CancellationToken ct)
    {
        await SendAsync(transactions.Select(transaction =>
            new TransactionResponse
            {
                Id = transaction.Id.Value,
                RecipientId = transaction.RecipientId.Value,
                SenderId = transaction.SenderId?.Value ?? Optional<string>.Empty(),
                ApplicationId = transaction.ApplicationId.Value,
                IsTransfer = transaction.SenderId.HasValue,
                Description = transaction.Description,
                Amount = transaction.Amount,
                SentAt = transaction.SentAt
            }).ToList(), cancellation: ct);
    }
}