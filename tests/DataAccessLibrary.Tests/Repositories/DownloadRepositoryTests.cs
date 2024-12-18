using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories.EntityFramework;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Data.Tests.Repositories;

public class DownloadRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();

    private IServiceCollection GetServiceCollection() => new ServiceCollection()
        .AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(_postgres.GetConnectionString()); })
        .AddScoped<DownloadRepository>();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDownload()
    {
        var faker = new Faker();

        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var createdAt = DateTime.UtcNow;

        var download = new DownloadDto
        {
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            CreatedAt = createdAt
        };

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.CreateAsync(download, default);

        result.Id.Should().NotBeEmpty();
        result.OwnerId.Should().Be(ownerId);
        result.PublicId.Should().Be(url);
        result.Title.Should().Be(title);
        result.Summary.Should().Be(summary);
        result.Description.Should().Be(description);
        result.Visibility.Should().Be(visibility);
        result.Verification.Should().Be(Verification.None);
        result.Version.Should().BeNull();
        result.CreatedAt.Should().Be(createdAt);

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var set = await dbContext.Set<DownloadDto>().ToListAsync();

        set.Should().HaveCount(1);
        set.First().Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetPublicAsync_ShouldIgnoreDeleted()
    {
        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, _ => Visibility.Public)
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.System.Version().ToString())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.DeletedAt, (f, x) => f.Date.Between(x.CreatedAt ?? f.Date.Past(), DateTime.UtcNow));

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Set<DownloadDto>().AddRange(faker.Generate(2));

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetPublicAsync(default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPublicAsync_ShouldIgnoreNonPublic()
    {
        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, f => f.PickRandom(Visibility.Private, Visibility.Unlisted))
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.System.Version().ToString())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Recent()));

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Set<DownloadDto>().AddRange(faker.Generate(2));

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetPublicAsync(default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPublicAsync_ShouldReturnPublic()
    {
        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Recent()))
            .RuleSet("public", r => r
                .RuleFor(x => x.Visibility, _ => Visibility.Public))
            .RuleSet("non_public", r => r
                .RuleFor(x => x.Visibility, f => f.PickRandom(Visibility.Private, Visibility.Unlisted)));

        DownloadDto[] collection =
        [
            ..faker.Generate(10, "public,default"),
            ..faker.Generate(10, "non_public,default")
        ];

        Random.Shared.Shuffle(collection);

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Set<DownloadDto>().AddRange(collection);

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetPublicAsync(default);

        result.Should().HaveCount(10);
        result.All(x => x.Visibility == Visibility.Public).Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserAsync_ShouldIgnoreDeleted()
    {
        var userId = Guid.NewGuid();

        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>())
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Recent()))
            .RuleFor(x => x.DeletedAt, (f, x) => f.Date.Between(x.CreatedAt ?? f.Date.Recent(), DateTime.UtcNow))
            .RuleSet("owned", r => r
                .RuleFor(x => x.OwnerId, _ => userId))
            .RuleSet("author", r => r
                .RuleFor(x => x.OwnerId, f => f.Random.Guid())
                .RuleFor(x => x.Authors, (f, x) =>
                [
                    new DownloadAuthorDto
                    {
                        DownloadId = x.Id,
                        UserId = userId,
                        CreatedAt = f.Date.Past()
                    }
                ]));

        DownloadDto[] collection =
        [
            ..faker.Generate(10, "owned,default"),
            ..faker.Generate(10, "author,default")
        ];

        Random.Shared.Shuffle(collection);

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Set<DownloadDto>().AddRange(collection);

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByUserAsync(userId, default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnOwnedDownloads()
    {
        var userId = Guid.NewGuid();

        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>())
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Recent()))
            .RuleSet("owned", r => r
                .RuleFor(x => x.OwnerId, _ => userId))
            .RuleSet("not_owned", r => r
                .RuleFor(x => x.OwnerId, f => f.Random.Guid()));

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        DownloadDto[] collection =
        [
            ..faker.Generate(10, "owned,default"),
            ..faker.Generate(10, "not_owned,default")
        ];

        Random.Shared.Shuffle(collection);

        dbContext.Set<DownloadDto>().AddRange(collection);

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByUserAsync(userId, default);

        result.Should().HaveCount(10);
        result.All(x => x.OwnerId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnDownloadsForAuthor()
    {
        var userId = Guid.NewGuid();

        var faker = new Faker<DownloadDto>()
            .UseDateTimeReference(DateTime.UtcNow)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.Random.Guid())
            .RuleFor(x => x.PublicId, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>())
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Recent()))
            .RuleSet("author", r => r
                .RuleFor(x => x.Authors, (f, x) =>
                [
                    new DownloadAuthorDto
                    {
                        DownloadId = x.Id,
                        UserId = userId,
                        CreatedAt = f.Date.Past()
                    }
                ]));

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        DownloadDto[] collection =
        [
            ..faker.Generate(10, "author,default"),
            ..faker.Generate(10)
        ];

        Random.Shared.Shuffle(collection);

        dbContext.Set<DownloadDto>().AddRange(collection);

        await dbContext.SaveChangesAsync();

        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByUserAsync(userId, default);

        result.Should().HaveCount(10);
        result.All(x => x.Authors.Any(a => a.UserId == userId)).Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnEmptyList_WhenUserIsNotAnAuthorOfAnyDownloads()
    {
        var userId = Guid.NewGuid();

        var provider = GetServiceCollection().BuildServiceProvider(true);

        using var scope = provider.CreateScope();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();
        
        var result = await repository.GetByUserAsync(userId, default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIgnoreDeleted()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var createdAt = faker.Date.Past();
        var deletedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            CreatedAt = createdAt,
            DeletedAt = deletedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByIdAsync(downloadId, default);
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenDownloadDoesNotExist()
    {
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();
        
        var result = await repository.GetByIdAsync(Guid.NewGuid(), default);
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDownload_WhenDownloadExists()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.System.Version().ToString();
        var createdAt = faker.Date.Past();
        var updatedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByIdAsync(downloadId, default);

        result.Should().NotBeNull();
        
        result?.Id.Should().Be(downloadId);
        result?.OwnerId.Should().Be(ownerId);
        result?.PublicId.Should().Be(url);
        result?.Title.Should().Be(title);
        result?.Summary.Should().Be(summary);
        result?.Description.Should().Be(description);
        result?.Visibility.Should().Be(visibility);
        result?.Verification.Should().Be(verification);
        result?.Version.Should().Be(version);
        result?.CreatedAt.Should().Be(createdAt);
        result?.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task GetByUrlAsync_ShouldIgnoreDeleted()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var createdAt = faker.Date.Past();
        var deletedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            CreatedAt = createdAt,
            DeletedAt = deletedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByPublicIdAsync(url, default);
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_ShouldReturnNull_WhenDownloadDoesNotExist()
    {
        var faker = new Faker();
        
        var url = faker.Random.AlphaNumeric(11);
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();
        
        var result = await repository.GetByPublicIdAsync(url, default);
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_ShouldReturnDownload_WhenDownloadExists()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.System.Version().ToString();
        var createdAt = faker.Date.Past();
        var updatedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        var result = await repository.GetByPublicIdAsync(url, default);

        result.Should().NotBeNull();
        
        result?.Id.Should().Be(downloadId);
        result?.OwnerId.Should().Be(ownerId);
        result?.PublicId.Should().Be(url);
        result?.Title.Should().Be(title);
        result?.Summary.Should().Be(summary);
        result?.Description.Should().Be(description);
        result?.Visibility.Should().Be(visibility);
        result?.Verification.Should().Be(verification);
        result?.Version.Should().Be(version);
        result?.CreatedAt.Should().Be(createdAt);
        result?.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDownload()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.System.Version().ToString();
        var createdAt = faker.Date.Past();
        var updatedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var updatedTitle = faker.Commerce.ProductName();
        var updatedSummary = faker.Lorem.Paragraph();
        var updatedDescription = faker.Lorem.Paragraphs();
        var updatedVisibility = faker.PickRandom<Visibility>();
        var updatedVersion = faker.System.Version().ToString();
        var now = DateTime.UtcNow;
        
        download.Title = updatedTitle;
        download.Summary = updatedSummary;
        download.Description = updatedDescription;
        download.Visibility = updatedVisibility;
        download.Version = updatedVersion;
        download.UpdatedAt = now;
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        await repository.UpdateAsync(download, default);

        await dbContext.Entry(download).ReloadAsync();
        
        download.Title.Should().Be(updatedTitle);
        download.Summary.Should().Be(updatedSummary);
        download.Description.Should().Be(updatedDescription);
        download.Visibility.Should().Be(updatedVisibility);
        download.Version.Should().Be(updatedVersion);
        download.UpdatedAt.Should().BeWithin(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkAsDeleted()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.System.Version().ToString();
        var createdAt = faker.Date.Past();
        var updatedAt = faker.Date.Recent();

        var download = new DownloadDto
        {
            Id = downloadId,
            OwnerId = ownerId,
            PublicId = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        
        var provider = GetServiceCollection().BuildServiceProvider(true);
        
        using var scope = provider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        dbContext.Set<DownloadDto>().Add(download);

        await dbContext.SaveChangesAsync();
        
        var repository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

        await repository.DeleteAsync(downloadId, default);

        await dbContext.Entry(download).ReloadAsync();

        download.DeletedAt.Should().NotBeNull().And.BeWithin(TimeSpan.FromSeconds(2));
    }
}