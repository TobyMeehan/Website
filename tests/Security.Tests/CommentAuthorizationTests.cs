using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Security;
using TobyMeehan.Com.Security.Handlers.Comments;

namespace Security.Tests;

public class CommentAuthorizationTests : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IAuthorizationService _authorizationService;

    public CommentAuthorizationTests()
    {
        var services = new ServiceCollection()
            .AddLogging();

        services.AddAuthorizationBuilder().RegisterPolicies();

        services.AddScoped<IAuthorizationHandler, PublicHandler>();
        services.AddScoped<IAuthorizationHandler, UserHandler>();

        var provider = services.BuildServiceProvider(true);

        _scope = provider.CreateScope();

        _authorizationService = _scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    private static Comment FakeComment(Guid? userId = null)
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        return new Comment
        {
            Id = faker.Random.Guid(),
            UserId = userId ?? faker.Random.Guid(),
            Content = faker.Lorem.Paragraph(),
            CreatedAt = faker.Date.Past(),
            EditedAt = faker.Date.Recent()
        };
    }

    [Fact]
    public async Task ViewComment_ShouldSucceed()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        var comment = FakeComment();

        var result = await _authorizationService.AuthorizeAsync(principal, comment, Policies.ViewComment);

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData(Policies.EditComment)]
    [InlineData(Policies.DeleteComment)]
    public async Task CommentOperation_ShouldFail_WhenUserIsNotAuthenticated(string policy)
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        var comment = FakeComment();

        var result = await _authorizationService.AuthorizeAsync(principal, comment, policy);

        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(Policies.EditComment)]
    [InlineData(Policies.DeleteComment)]
    public async Task CommentOperation_ShouldFail_WhenUserDidNotCreateComment(string policy)
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(CommentAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            ]);

        var principal = new ClaimsPrincipal(identity);

        var comment = FakeComment();

        var result = await _authorizationService.AuthorizeAsync(principal, comment, policy);

        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(Policies.EditComment)]
    [InlineData(Policies.DeleteComment)]
    public async Task CommentOperation_ShouldSucceed_WhenUserCreatedComment(string policy)
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(CommentAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            ]);

        var principal = new ClaimsPrincipal(identity);

        var comment = FakeComment(userId: userId);

        var result = await _authorizationService.AuthorizeAsync(principal, comment, policy);

        result.Succeeded.Should().BeTrue();
    }
}