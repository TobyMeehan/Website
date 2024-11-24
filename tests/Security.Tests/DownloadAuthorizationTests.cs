using System.Security.Claims;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;
using TobyMeehan.Com.Security;
using TobyMeehan.Com.Security.Handlers.Downloads;

namespace Security.Tests;

public class DownloadAuthorizationTests : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IDownloadAuthorService _downloadAuthorService = A.Fake<IDownloadAuthorService>();

    private readonly IAuthorizationService _authorizationService;

    public DownloadAuthorizationTests()
    {
        var services = new ServiceCollection()
            .AddLogging();

        services.AddScoped<IDownloadAuthorService>(_ => _downloadAuthorService);

        services.AddAuthorizationBuilder().RegisterPolicies();

        services.AddScoped<IAuthorizationHandler, PublicHandler>();
        services.AddScoped<IAuthorizationHandler, OwnerHandler>();
        services.AddScoped<IAuthorizationHandler, AuthorHandler>();

        var provider = services.BuildServiceProvider(true);

        _scope = provider.CreateScope();

        _authorizationService = _scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    private static Download FakeDownload(
        Visibility? visibility = null)
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        return new Download
        {
            Id = faker.Random.Guid(),
            Url = faker.Random.AlphaNumeric(11),
            Title = faker.Hacker.Phrase(),
            Summary = faker.Lorem.Paragraph(),
            Description = faker.Lorem.Paragraphs(),
            Visibility = visibility ?? faker.PickRandom<Visibility>(),
            Verification = faker.PickRandom<Verification>(),
            Version = faker.PickRandom(null, faker.System.Version()),
            CreatedAt = faker.Date.Past(),
            UpdatedAt = faker.PickRandom(null as DateTime?, faker.Date.Recent()),
        };
    }

    [Fact]
    public async Task ViewPublicDownload_ShouldSucceed_WhenUserIsNotAuthenticated()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Public);

        var result = await _authorizationService.AuthorizeAsync(principal, download, Policies.ViewDownload);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ViewPrivateDownload_ShouldFail_WhenUserIsNotAuthenticated()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Private);

        var result = await _authorizationService.AuthorizeAsync(principal, download, Policies.ViewDownload);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ViewPrivateDownload_ShouldFail_WhenUserIsNotAuthor()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Private);

        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._))
            .Returns(false);

        var result = await _authorizationService.AuthorizeAsync(principal, download, Policies.ViewDownload);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ViewPrivateDownload_ShouldSucceed_WhenUserIsOwner()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Private);

        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._))
            .Returns(false);

        var result = await _authorizationService.AuthorizeAsync(principal, download, Policies.ViewDownload);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ViewPrivateDownload_ShouldSucceed_WhenUserIsAuthor()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);
        
        var principal = new ClaimsPrincipal(identity);
        
        var download = FakeDownload(visibility: Visibility.Private);
        
        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._)).Returns(true);
        
        var result = await _authorizationService.AuthorizeAsync(principal, download, Policies.ViewDownload);
        
        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData(Policies.ManageDownload)]
    [InlineData(Policies.EditDownload)]
    [InlineData(Policies.DeleteDownload)]
    [InlineData(Policies.UploadFile)]
    [InlineData(Policies.DeleteFile)]
    [InlineData(Policies.InviteAuthor)]
    [InlineData(Policies.KickAuthor)]
    public async Task ManageDownload_ShouldFail_WhenUserIsNotAuthenticated(string policy)
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        
        var download = FakeDownload();
        
        var result = await _authorizationService.AuthorizeAsync(principal, download, policy);
        
        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(Policies.ManageDownload)]
    [InlineData(Policies.EditDownload)]
    [InlineData(Policies.DeleteDownload)]
    [InlineData(Policies.UploadFile)]
    [InlineData(Policies.DeleteFile)]
    [InlineData(Policies.InviteAuthor)]
    [InlineData(Policies.KickAuthor)]
    public async Task ManageDownload_ShouldFail_WhenUserIsNotAuthor(string policy)
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload();

        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._))
            .Returns(false);

        var result = await _authorizationService.AuthorizeAsync(principal, download, policy);

        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(Policies.ManageDownload)]
    [InlineData(Policies.EditDownload)]
    [InlineData(Policies.DeleteDownload)]
    [InlineData(Policies.UploadFile)]
    [InlineData(Policies.DeleteFile)]
    [InlineData(Policies.InviteAuthor)]
    [InlineData(Policies.KickAuthor)]
    public async Task ManageDownload_ShouldSucceed_WhenUserIsOwner(string policy)
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Private);

        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._))
            .Returns(false);

        var result = await _authorizationService.AuthorizeAsync(principal, download, policy);

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData(Policies.ManageDownload)]
    [InlineData(Policies.EditDownload)]
    [InlineData(Policies.DeleteDownload)]
    [InlineData(Policies.UploadFile)]
    [InlineData(Policies.DeleteFile)]
    [InlineData(Policies.InviteAuthor)]
    [InlineData(Policies.KickAuthor)]
    public async Task ManageDownload_ShouldSucceed_WhenUserIsAuthor(string policy)
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);

        var download = FakeDownload(visibility: Visibility.Private);

        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(download.Id, userId, A<CancellationToken>._)).Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(download.Id, userId, A<CancellationToken>._))
            .Returns(true);

        var result = await _authorizationService.AuthorizeAsync(principal, download, policy);

        result.Succeeded.Should().BeTrue();
    }
}