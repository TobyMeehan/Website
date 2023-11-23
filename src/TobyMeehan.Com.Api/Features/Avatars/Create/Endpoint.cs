using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Avatar;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Avatars.Create;

public class Endpoint : Endpoint<Request, AvatarResponse>
{
    private readonly IAvatarService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IAvatarService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Post("/users/{UserId}/avatars");
        Policies(PolicyNames.User.Scope.Update);
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Update);

        if (authorizationResult.Succeeded)
        {
            await CreateAsync(user, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task CreateAsync(IUser user, Request req, CancellationToken ct)
    {
        var avatar = await _service.CreateAsync(new CreateAvatarBuilder()
                
                .WithUser(user.Id)
            
                .WithFile(new FileUploadBuilder()
                    .WithContentType(MediaType.Parse(req.Avatar.ContentType))
                    .WithSize(req.Avatar.Length)
                    .WithStream(req.Avatar.OpenReadStream())), 
            
            cancellationToken: ct);

        await SendCreatedAtAsync<Get.Endpoint>(new { UserId = user.Id, Id = avatar.Id.Value }, responseBody:
            new AvatarResponse
            {
                Id = avatar.Id.Value
            }, cancellation: ct);
    }
}