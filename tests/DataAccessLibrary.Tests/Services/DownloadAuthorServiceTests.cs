using Bogus;
using FakeItEasy;
using FluentAssertions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;

namespace TobyMeehan.Com.Data.Tests.Services;

public class DownloadAuthorServiceTests
{
    private readonly DownloadAuthorService _sut;
    private readonly IDownloadAuthorRepository _downloadAuthorRepository = A.Fake<IDownloadAuthorRepository>();
    private readonly IDownloadRepository _downloadRepository = A.Fake<IDownloadRepository>();

    public DownloadAuthorServiceTests()
    {
        _sut = new DownloadAuthorService(_downloadAuthorRepository, _downloadRepository);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAuthor_WhenAuthorExists()
    {
        var faker = new Faker();

        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var createdAt = faker.Date.Past();

        var downloadAuthorDto = new DownloadAuthorDto
        {
            DownloadId = downloadId,
            UserId = userId,
            CreatedAt = createdAt,
        };

        A.CallTo(() => _downloadAuthorRepository.GetAsync(downloadId, userId, A<CancellationToken>._))
            .Returns(downloadAuthorDto);

        var result = await _sut.AddAsync(downloadId, userId);

        result.Should().NotBeNull();
        
        result.DownloadId.Should().Be(downloadId);
        result.UserId.Should().Be(userId);
        result.CreatedAt.Should().Be(createdAt);
        result.IsOwner.Should().BeFalse();
        
        A.CallTo(() => _downloadAuthorRepository.AddAsync(A<DownloadAuthorDto>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task AddAsync_ShouldCreateAuthor_WhenAuthorDoesNotExist()
    {
        var faker = new Faker();
        
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();

        A.CallTo(() => _downloadAuthorRepository.GetAsync(downloadId, userId, A<CancellationToken>._))
            .Returns<DownloadAuthorDto?>(null);

        var capturedDto = A.Captured<DownloadAuthorDto>();

        A.CallTo(() => _downloadAuthorRepository.AddAsync(capturedDto._, A<CancellationToken>._))
            .ReturnsLazily((DownloadAuthorDto input, CancellationToken _) => new DownloadAuthorDto
            {
                DownloadId = input.DownloadId,
                UserId = input.UserId,
                CreatedAt = input.CreatedAt,
            });
        
        var result = await _sut.AddAsync(downloadId, userId);

        var captured = capturedDto.GetLastValue();
        
        captured.DownloadId.Should().Be(downloadId);
        captured.UserId.Should().Be(userId);
        captured.CreatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));

        result.Should().NotBeNull();
        
        result.DownloadId.Should().Be(downloadId);
        result.UserId.Should().Be(userId);
        result.CreatedAt.Should().Be(captured.CreatedAt);
    }

    [Fact]
    public async Task GetByDownloadAsync_ShouldReturnEmptyList_WhenDownloadDoesNotExist()
    {
        var downloadId = Guid.NewGuid();
        
        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns<DownloadDto?>(null);
        
        var result = await _sut.GetByDownloadAsync(downloadId);
        
        result.Should().BeEmpty();
        
        A.CallTo(() => _downloadAuthorRepository.GetByDownloadAsync(downloadId, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task GetByDownloadAsync_ShouldReturnOnlyOwner_WhenDownloadHasNoAuthors()
    {
        var faker = new Faker();

        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var createdAt = faker.Date.Past();

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns(new DownloadDto { Id = downloadId, OwnerId = ownerId, CreatedAt = createdAt });

        A.CallTo(() => _downloadAuthorRepository.GetByDownloadAsync(downloadId, A<CancellationToken>._))
            .Returns([]);
        
        var result = await _sut.GetByDownloadAsync(downloadId);

        result.Should().HaveCount(1);
        
        result[0].DownloadId.Should().Be(downloadId);
        result[0].UserId.Should().Be(ownerId);
        result[0].IsOwner.Should().BeTrue();
        result[0].CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async Task GetByDownloadAsync_ShouldReturnCollection_WhenDownloadHasAuthors()
    {
        var faker = new Faker();
        
        var downloadId = faker.Random.Guid();
        var ownerId = faker.Random.Guid();
        var createdAt = faker.Date.Past();

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns(new DownloadDto { Id = downloadId, OwnerId = ownerId, CreatedAt = createdAt });
        
        var authorFaker = new Faker<DownloadAuthorDto>()
            .RuleFor(x => x.DownloadId, _ => downloadId)
            .RuleFor(x => x.UserId, f => f.Random.Guid())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past());
        
        var collection = authorFaker.Generate(20);
        
        A.CallTo(() => _downloadAuthorRepository.GetByDownloadAsync(downloadId, A<CancellationToken>._))
            .Returns(collection);
        
        var results = await _sut.GetByDownloadAsync(downloadId);
        
        results.Should().HaveCount(collection.Count + 1);
        results.Should().ContainSingle(x => x.IsOwner);

        foreach (var (i, author) in results.Skip(1).Select((x, i) => (i,x)))
        {
            author.DownloadId.Should().Be(downloadId);
            author.UserId.Should().Be(collection[i].UserId);
            author.CreatedAt.Should().Be(collection[i].CreatedAt);
            author.IsOwner.Should().BeFalse();
        }
    }

    [Fact]
    public async Task IsAuthorAsync_ShouldReturnFalse_WhenUserIsNotAnAuthor()
    {
        var downloadId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        A.CallTo(() => _downloadAuthorRepository.GetAsync(downloadId, userId, A<CancellationToken>._))
            .Returns<DownloadAuthorDto?>(null);
        
        var result = await _sut.IsAuthorAsync(downloadId, userId);
        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAuthorAsync_ShouldReturnTrue_WhenUserIsAuthor()
    {
        var faker = new Faker
        {
            Date =
            {
                LocalSystemClock = () => DateTime.UtcNow
            }
        };
        
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var createdAt = faker.Date.Past();

        var author = new DownloadAuthorDto
        {
            DownloadId = downloadId,
            UserId = userId,
            CreatedAt = createdAt
        };

        A.CallTo(() => _downloadAuthorRepository.GetAsync(downloadId, userId, A<CancellationToken>._))
            .Returns(author);
        
        var result = await _sut.IsAuthorAsync(downloadId, userId);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsOwnerAsync_ShouldReturnFalse_WhenDownloadDoesNotExist()
    {
        var downloadId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns<DownloadDto?>(null);
        
        var result = await _sut.IsOwnerAsync(downloadId, userId);
        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsOwnerAsync_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        var downloadId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns(new DownloadDto { Id = downloadId, OwnerId = ownerId });
        
        var result = await _sut.IsOwnerAsync(downloadId, userId);
        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsOwnerAsync_ShouldReturnTrue_WhenUserIsOwner()
    {
        var downloadId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        A.CallTo(() => _downloadRepository.GetByIdAsync(downloadId, A<CancellationToken>._))
            .Returns(new DownloadDto { Id = downloadId, OwnerId = userId });
        
        var result = await _sut.IsOwnerAsync(downloadId, userId);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallRepositoryMethod()
    {
        var downloadId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _sut.RemoveAsync(downloadId, userId);
        
        A.CallTo(() => _downloadAuthorRepository.RemoveAsync(downloadId, userId, A<CancellationToken>._))
            .MustHaveHappened();
    }
}