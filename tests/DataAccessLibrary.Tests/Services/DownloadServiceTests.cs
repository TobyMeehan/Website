using Bogus;
using FakeItEasy;
using FluentAssertions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Tests.Services;

public class DownloadServiceTests
{
    private readonly DownloadService _sut;
    private readonly IDownloadRepository _downloadRepository = A.Fake<IDownloadRepository>();

    public DownloadServiceTests()
    {
        _sut = new DownloadService(_downloadRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepository_WithCorrectData()
    {
        var faker = new Faker();

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();

        var capturedDto = A.Captured<DownloadDto>();

        A.CallTo(() => _downloadRepository.CreateAsync(capturedDto._, A<CancellationToken>._))
            .ReturnsLazily((DownloadDto input, CancellationToken _) => new DownloadDto
            {
                Id = downloadId,
                OwnerId = input.OwnerId,
                Url = input.Url,
                Title = input.Title,
                Summary = input.Summary,
                Description = input.Description,
                Visibility = input.Visibility,
                Verification = input.Verification,
                Version = input.Version,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt
            });

        var create = new IDownloadService.CreateDownload(ownerId, title, summary, description, visibility);
        
        await _sut.CreateAsync(create);

        var captured = capturedDto.GetLastValue();

        captured.OwnerId.Should().Be(ownerId);
        captured.Url.Should().NotBeNull();
        captured.Title.Should().Be(title);
        captured.Summary.Should().Be(summary);
        captured.Description.Should().Be(description);
        captured.Visibility.Should().Be(visibility);
        captured.Verification.Should().Be(Verification.None);
        captured.Version.Should().BeNull();
        captured.CreatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
        captured.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDownload_WithMappedData()
    {
        var faker = new Faker();

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();

        A.CallTo(() => _downloadRepository.CreateAsync(A<DownloadDto>._, A<CancellationToken>._))
            .ReturnsLazily((DownloadDto input, CancellationToken _) => new DownloadDto
            {
                Id = downloadId,
                OwnerId = input.OwnerId,
                Url = input.Url,
                Title = input.Title,
                Summary = input.Summary,
                Description = input.Description,
                Visibility = input.Visibility,
                Verification = input.Verification,
                Version = input.Version,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt
            });

        var create = new IDownloadService.CreateDownload(ownerId, title, summary, description, visibility);
        
        var download = await _sut.CreateAsync(create);

        download.Id.Should().Be(downloadId);
        download.Url.Should().NotBeNull();
        download.Title.Should().Be(title);
        download.Summary.Should().Be(summary);
        download.Description.Should().Be(description);
        download.Visibility.Should().Be(visibility);
        download.Verification.Should().Be(Verification.None);
        download.Version.Should().BeNull();
        download.CreatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
        download.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetPublicAsync_ShouldReturnCollection_WhenCollectionExists()
    {
        var faker = new Faker<DownloadDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.Random.Guid())
            .RuleFor(x => x.Url, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, _ => Visibility.Public)
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Past()));

        var collection = faker.Generate(20);

        A.CallTo(() => _downloadRepository.GetPublicAsync(A<CancellationToken>._)).Returns(collection);

        var result = await _sut.GetPublicAsync();

        result.Should().HaveCount(collection.Count);

        foreach (var (i, download) in result.Select((x, i) => (i, x)))
        {
            download.Id.Should().Be(collection[i].Id);
            download.Url.Should().Be(collection[i].Url);
            download.Title.Should().Be(collection[i].Title);
            download.Summary.Should().Be(collection[i].Summary);
            download.Description.Should().Be(collection[i].Description);
            download.Visibility.Should().Be(Visibility.Public);
            download.Verification.Should().Be(collection[i].Verification);
            download.Version?.ToString().Should().Be(collection[i].Version);
            download.CreatedAt.Should().Be(collection[i].CreatedAt);
            download.UpdatedAt.Should().Be(collection[i].UpdatedAt);
        }
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnCollection_WhenCollectionExists()
    {
        var userId = Guid.NewGuid();

        var faker = new Faker<DownloadDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.OwnerId, f => f.PickRandom(userId, f.Random.Guid()))
            .RuleFor(x => x.Url, f => f.Random.AlphaNumeric(11))
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Summary, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Description, f => f.Lorem.Paragraphs())
            .RuleFor(x => x.Visibility, f => f.PickRandom<Visibility>())
            .RuleFor(x => x.Verification, f => f.PickRandom<Verification>())
            .RuleFor(x => x.Version, f => f.PickRandom(null, f.System.Version().ToString()))
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.UpdatedAt, f => f.PickRandom(null as DateTime?, f.Date.Past()));

        var collection = faker.Generate(20);

        A.CallTo(() => _downloadRepository.GetByUserAsync(userId, A<CancellationToken>._)).Returns(collection);

        var result = await _sut.GetByUserAsync(userId);

        result.Should().HaveCount(collection.Count);

        foreach (var (i, download) in result.Select((x, i) => (i, x)))
        {
            download.Id.Should().Be(collection[i].Id);
            download.Url.Should().Be(collection[i].Url);
            download.Title.Should().Be(collection[i].Title);
            download.Summary.Should().Be(collection[i].Summary);
            download.Description.Should().Be(collection[i].Description);
            download.Visibility.Should().Be(collection[i].Visibility);
            download.Verification.Should().Be(collection[i].Verification);
            download.Version?.ToString().Should().Be(collection[i].Version);
            download.CreatedAt.Should().Be(collection[i].CreatedAt);
            download.UpdatedAt.Should().Be(collection[i].UpdatedAt);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDownload_WhenDownloadExists()
    {
        // Arrange

        var faker = new Faker();

        var downloadId = Guid.NewGuid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.PickRandom(null, faker.System.Version());
        var createdAt = faker.Date.Past();
        var updatedAt = faker.PickRandom(null as DateTime?, faker.Date.Past());

        var downloadDto = new DownloadDto
        {
            Id = downloadId,
            Url = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version?.ToString(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._)).Returns(downloadDto);

        // Act

        var download = await _sut.GetByIdAsync(downloadId);

        // Assert
        download.Should().NotBeNull();

        download?.Id.Should().Be(downloadId);
        download?.Url.Should().Be(url);
        download?.Title.Should().Be(title);
        download?.Summary.Should().Be(summary);
        download?.Description.Should().Be(description);
        download?.Visibility.Should().Be(visibility);
        download?.Verification.Should().Be(verification);
        download?.Version.Should().Be(version);
        download?.CreatedAt.Should().Be(createdAt);
        download?.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenDownloadDoesNotExist()
    {
        var downloadId = Guid.NewGuid();

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns<DownloadDto?>(null);

        var download = await _sut.GetByIdAsync(downloadId);

        download.Should().BeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_ShouldReturnDownload_WhenDownloadExists()
    {
        var faker = new Faker();

        var downloadId = Guid.NewGuid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.PickRandom(null, faker.System.Version());
        var createdAt = faker.Date.Past();
        var updatedAt = faker.PickRandom(null as DateTime?, faker.Date.Past());

        var downloadDto = new DownloadDto
        {
            Id = downloadId,
            Url = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version?.ToString(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };

        A.CallTo(() => _downloadRepository.GetByUrlAsync(url, A<CancellationToken>._)).Returns(downloadDto);

        var download = await _sut.GetByUrlAsync(url);

        download.Should().NotBeNull();

        download?.Id.Should().Be(downloadId);
        download?.Url.Should().Be(url);
        download?.Title.Should().Be(title);
        download?.Summary.Should().Be(summary);
        download?.Description.Should().Be(description);
        download?.Visibility.Should().Be(visibility);
        download?.Verification.Should().Be(verification);
        download?.Version.Should().Be(version);
        download?.CreatedAt.Should().Be(createdAt);
        download?.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task GetByUrlAsync_ShouldReturnNull_WhenDownloadDoesNotExist()
    {
        var faker = new Faker();

        var url = faker.Random.AlphaNumeric(11);

        A.CallTo(() => _downloadRepository.GetByUrlAsync(url, A<CancellationToken>._))
            .Returns<DownloadDto?>(null);

        var download = await _sut.GetByUrlAsync(url);

        download.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDownload_WhenDownloadExists()
    {
        var faker = new Faker();

        var downloadId = Guid.NewGuid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var version = faker.PickRandom(null, faker.System.Version());
        var createdAt = faker.Date.Past();
        var updatedAt = faker.PickRandom(null as DateTime?, faker.Date.Past());

        var downloadDto = new DownloadDto
        {
            Id = downloadId,
            Url = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = version?.ToString(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._)).Returns(downloadDto);

        var capturedDto = A.Captured<DownloadDto>();

        A.CallTo(() =>
                _downloadRepository.UpdateAsync(capturedDto.That.Matches(x => x.Id == downloadId),
                    A<CancellationToken>._))
            .ReturnsLazily((DownloadDto input, CancellationToken _) => new DownloadDto
            {
                Id = downloadId,
                Url = input.Url,
                Title = input.Title,
                Summary = input.Summary,
                Description = input.Description,
                Visibility = input.Visibility,
                Verification = input.Verification,
                Version = input.Version,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt
            });

        var updatedTitle = faker.Commerce.ProductName();
        var updatedSummary = faker.Lorem.Paragraph();
        var updatedDescription = faker.Lorem.Paragraphs();
        var updatedVisibility = faker.PickRandom<Visibility>();

        var update = new IDownloadService.UpdateDownload(updatedTitle, updatedSummary, updatedDescription,
            updatedVisibility, version);

        var result = await _sut.UpdateAsync(downloadId, update);

        var captured = capturedDto.GetLastValue();

        captured.Title.Should().Be(updatedTitle);
        captured.Summary.Should().Be(updatedSummary);
        captured.Description.Should().Be(updatedDescription);
        captured.Visibility.Should().Be(updatedVisibility);
        captured.Version.Should().Be(downloadDto.Version);

        result.Should().NotBeNull();

        result?.Id.Should().Be(downloadId);
        result?.Url.Should().Be(url);
        result?.Title.Should().Be(updatedTitle);
        result?.Summary.Should().Be(updatedSummary);
        result?.Description.Should().Be(updatedDescription);
        result?.Visibility.Should().Be(updatedVisibility);
        result?.Verification.Should().Be(verification);
        result?.Version.Should().Be(version);
        result?.CreatedAt.Should().Be(createdAt);
        result?.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangeUpdatedAt_WhenVersionIsIncreased()
    {
        var faker = new Faker();

        var downloadId = Guid.NewGuid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var currentVersion = new Version(1, 0);
        var createdAt = faker.Date.Past();
        var updatedAt = faker.PickRandom(null as DateTime?, faker.Date.Past());

        var downloadDto = new DownloadDto
        {
            Id = downloadId,
            Url = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = currentVersion.ToString(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._)).Returns(downloadDto);

        var capturedDto = A.Captured<DownloadDto>();

        A.CallTo(() =>
            _downloadRepository.UpdateAsync(capturedDto.That.Matches(x => x.Id == downloadId),
                A<CancellationToken>._)).Returns(A.Dummy<DownloadDto>());

        var updatedVersion = new Version(2, 0);

        var update = new IDownloadService.UpdateDownload(title, summary, description, visibility, updatedVersion);

        await _sut.UpdateAsync(downloadId, update);

        var captured = capturedDto.GetLastValue();

        captured.UpdatedAt.Should().NotBeNull("The download should be marked as updated.");
        captured.UpdatedAt.Should().NotBe(updatedAt);
        captured.UpdatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangeUpdatedAt_WhenVersionIsSetFromNull()
    {
        var faker = new Faker();

        var downloadId = Guid.NewGuid();
        var url = faker.Random.AlphaNumeric(11);
        var title = faker.Commerce.ProductName();
        var summary = faker.Lorem.Paragraph();
        var description = faker.Lorem.Paragraphs();
        var visibility = faker.PickRandom<Visibility>();
        var verification = faker.PickRandom<Verification>();
        var createdAt = faker.Date.Past();
        var updatedAt = faker.PickRandom(null as DateTime?, faker.Date.Past());

        var downloadDto = new DownloadDto
        {
            Id = downloadId,
            Url = url,
            Title = title,
            Summary = summary,
            Description = description,
            Visibility = visibility,
            Verification = verification,
            Version = null,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._)).Returns(downloadDto);

        var capturedDto = A.Captured<DownloadDto>();

        A.CallTo(() =>
            _downloadRepository.UpdateAsync(capturedDto.That.Matches(x => x.Id == downloadId),
                A<CancellationToken>._)).Returns(A.Dummy<DownloadDto>());

        var updatedVersion = new Version(2, 0);

        var update = new IDownloadService.UpdateDownload(title, summary, description, visibility, updatedVersion);

        await _sut.UpdateAsync(downloadId, update);

        var captured = capturedDto.GetLastValue();

        captured.UpdatedAt.Should().NotBeNull("The download should be marked as updated.");
        captured.UpdatedAt.Should().NotBe(updatedAt);
        captured.UpdatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenDownloadDoesNotExist()
    {
        var downloadId = Guid.NewGuid();

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns<DownloadDto?>(null);

        var download = await _sut.UpdateAsync(downloadId, A.Dummy<IDownloadService.UpdateDownload>());

        download.Should().BeNull();

        A.CallTo(() => _downloadRepository.UpdateAsync(A<DownloadDto>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryMethod()
    {
        var downloadId = Guid.NewGuid();

        await _sut.DeleteAsync(downloadId);

        A.CallTo(() => _downloadRepository.DeleteAsync(downloadId, A<CancellationToken>._)).MustHaveHappened();
    }
}