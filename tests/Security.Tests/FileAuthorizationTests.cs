using System.Security.Claims;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;
using TobyMeehan.Com.Security;
using TobyMeehan.Com.Security.Handlers.Files;

namespace Security.Tests;

public class FileAuthorizationTests : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IDownloadAuthorService _downloadAuthorService = A.Fake<IDownloadAuthorService>();

    private readonly IAuthorizationService _authorizationService;
    
    public FileAuthorizationTests()
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

    private static DownloadFile FakeDownloadFile(Visibility? visibility = null)
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        return new DownloadFile
        {
            Id = faker.Random.Guid(),
            DownloadId = faker.Random.Guid(),
            Filename = faker.System.FileName(),
            ContentType = faker.System.MimeType(),
            SizeInBytes = faker.Random.Long(),
            Visibility = visibility ?? faker.PickRandom<Visibility>(),
            CreatedAt = faker.Date.Past(),
            UpdatedAt = faker.Date.Recent(),
        };
    }

    [Fact]
    public async Task ViewPublicFile_ShouldSucceed_WhenUserIsNotAuthenticated()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        
        var file = FakeDownloadFile(visibility: Visibility.Public);

        var result = await _authorizationService.AuthorizeAsync(principal, file, Policies.ViewFile);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ViewPrivateFile_ShouldFail_WhenUserIsNotAuthenticated()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        
        var file = FakeDownloadFile(visibility: Visibility.Private);
        
        var result = await _authorizationService.AuthorizeAsync(principal, file, Policies.ViewFile);
        
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ViewPrivateFile_ShouldFail_WhenUserIsNotAuthor()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);
        
        var file = FakeDownloadFile(visibility: Visibility.Private);
        
        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(false);
        
        var result = await _authorizationService.AuthorizeAsync(principal, file, Policies.ViewFile);
        
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ViewPrivateFile_ShouldSucceed_WhenUserIsOwner()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);
        
        var file = FakeDownloadFile(visibility: Visibility.Private);
        
        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(true);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(false);
        
        var result = await _authorizationService.AuthorizeAsync(principal, file, Policies.ViewFile);
        
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ViewPrivateFile_ShouldSucceed_WhenUserIsAuthor()
    {
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity(authenticationType: nameof(DownloadAuthorizationTests),
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]);

        var principal = new ClaimsPrincipal(identity);
        
        var file = FakeDownloadFile(visibility: Visibility.Private);
        
        A.CallTo(() => _downloadAuthorService.IsOwnerAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(false);
        A.CallTo(() => _downloadAuthorService.IsAuthorAsync(file.DownloadId, userId, A<CancellationToken>._))
            .Returns(true);
        
        var result = await _authorizationService.AuthorizeAsync(principal, file, Policies.ViewFile);
        
        result.Succeeded.Should().BeTrue();
    }
}