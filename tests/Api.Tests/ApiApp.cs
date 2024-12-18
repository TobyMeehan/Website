using System.Security.Claims;
using FastEndpoints.Security;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.LocalStack;
using Testcontainers.PostgreSql;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Domain.Thavyra;
using TobyMeehan.Com.Domain.Thavyra.Services;
using TobyMeehan.Com.Security.Configuration;

namespace Api.Tests;

public class ApiApp : AppFixture<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();
    private readonly LocalStackContainer _localStack = new LocalStackBuilder().Build();

    public User UserA { get; private set; } = null!;
    public User UserB { get; private set; } = null!;

    public HttpClient ClientA { get; private set; } = null!;
    public HttpClient ClientB { get; private set; } = null!;

    public List<string> Downloads { get; set; } = [];

    protected override async Task PreSetupAsync()
    {
        await _postgres.StartAsync();
        await _localStack.StartAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        var configurationValues = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = Guid.NewGuid().ToString(),
            ["Data:Postgres:ConnectionString"] = _postgres.GetConnectionString(),
            ["Data:S3:Bucket"] = "test",
            ["Data:S3:Configuration:ServiceUrl"] =
                $"http://{_localStack.Hostname}:{_localStack.GetMappedPublicPort(4566)}"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(c => { c.AddInMemoryCollection(configurationValues); });
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IUserService>(_ => new InMemoryUserService());
    }

    protected override async Task SetupAsync()
    {
        using var scope = Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>() as InMemoryUserService;
        var jwtOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        UserA = new User
        {
            Id = Fake.Random.Guid(),
            Username = Fake.Internet.UserName(),
            AvatarUrl = Fake.Internet.Avatar(),
            ProfileUrl = Fake.Internet.Url()
        };

        userService?.AddUser(UserA);

        var userAToken = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtOptions.Key!;
            options.ExpireAt = DateTime.UtcNow.AddHours(1);
            options.User[ClaimTypes.NameIdentifier] = UserA.Id.ToString();
        });

        ClientA = CreateClient(c => { c.DefaultRequestHeaders.Authorization = new("Bearer", userAToken); });

        UserB = new User
        {
            Id = Fake.Random.Guid(),
            Username = Fake.Internet.UserName(),
            AvatarUrl = Fake.Internet.Avatar(),
            ProfileUrl = Fake.Internet.Url()
        };

        userService?.AddUser(UserB);

        var userBToken = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtOptions.Key!;
            options.ExpireAt = DateTime.UtcNow.AddHours(1);
            options.User[ClaimTypes.NameIdentifier] = UserB.Id.ToString();
        });

        ClientB = CreateClient(c => { c.DefaultRequestHeaders.Authorization = new("Bearer", userBToken); });
    }

    protected override async Task TearDownAsync()
    {
        var userService = Services.GetRequiredService<IUserService>() as InMemoryUserService;

        userService?.Clear();

        ClientA?.Dispose();
        ClientB?.Dispose();

        await _postgres.StopAsync();
        await _localStack.StopAsync();
    }
}