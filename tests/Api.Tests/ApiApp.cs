using FakeItEasy;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Domain.Thavyra;
using TobyMeehan.Com.Domain.Thavyra.Services;
using TobyMeehan.Com.Features.Token.Post;
using TokenRequest = TobyMeehan.Com.Features.Token.Post.TokenRequest;
using TokenResponse = TobyMeehan.Com.Features.Token.Post.TokenResponse;

namespace Api.Tests;

public class ApiApp : AppFixture<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();
    
    private readonly IUserService _userService = A.Fake<IUserService>();
    private readonly Dictionary<string, User> _users = [];

    public HttpClient UserA { get; private set; } = null!;
    public HttpClient UserB { get; private set; } = null!;

    public List<string> Downloads { get; set; } = [];

    protected override async Task PreSetupAsync()
    {
        await _postgres.StartAsync();
    }

    protected override IHost ConfigureAppHost(IHostBuilder a)
    {
        a.ConfigureAppConfiguration(c =>
        {
            c.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Data:Postgres:ConnectionString"] = _postgres.GetConnectionString()
            });
        });

        return base.ConfigureAppHost(a);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserService>(_ => _userService);

        _users["UserA"] = new User
        {
            Id = Fake.Random.Guid(),
            Username = Fake.Internet.UserName(),
            AvatarUrl = Fake.Internet.Avatar(),
            ProfileUrl = Fake.Internet.Url()
        };

        _users["UserB"] = new User
        {
            Id = Fake.Random.Guid(),
            Username = Fake.Internet.UserName(),
            AvatarUrl = Fake.Internet.Avatar(),
            ProfileUrl = Fake.Internet.Url()
        };
        
        A.CallTo(() => _userService.GetByAccessTokenAsync(A<string>._, A<CancellationToken>._))
            .ReturnsLazily((string token, CancellationToken _) => _users[token]);
    }

    protected override async Task SetupAsync()
    {
        using var scope = Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.EnsureCreatedAsync();

        var userATokenResponse = await Client.POSTAsync<TokenEndpoint, TokenRequest, TokenResponse>(new()
        {
            AccessToken = "UserA"
        });

        UserA = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", userATokenResponse.Result.Token);
        });

        var userBTokenResponse = await Client.POSTAsync<TokenEndpoint, TokenRequest, TokenResponse>(new()
        {
            AccessToken = "UserB"
        });

        UserB = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", userBTokenResponse.Result.Token);
        });
    }

    protected override async Task TearDownAsync()
    {
        UserA.Dispose();
        
        await _postgres.StopAsync();
    }
}