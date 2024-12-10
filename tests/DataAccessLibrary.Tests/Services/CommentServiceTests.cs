using Bogus;
using FakeItEasy;
using FluentAssertions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Tests.Services;

public class CommentServiceTests
{
    private readonly CommentService _sut;
    private readonly ICommentRepository _commentRepository = A.Fake<ICommentRepository>();

    public CommentServiceTests()
    {
        _sut = new CommentService(_commentRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepository_WithCorrectData()
    {
        var faker = new Faker();

        var commentId = faker.Random.Guid();
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var content = faker.Lorem.Paragraph();

        var capturedDto = A.Captured<CommentDto>();

        A.CallTo(() => _commentRepository.CreateAsync(capturedDto._, A<CancellationToken>._))
            .ReturnsLazily((CommentDto input, CancellationToken _) => new CommentDto
            {
                Id = commentId,
                Download = new DownloadCommentDto { CommentId = commentId, DownloadId = downloadId },
                UserId = input.UserId,
                Content = input.Content,
                CreatedAt = input.CreatedAt,
                EditedAt = input.EditedAt
            });

        var download = new Download
        {
            Id = downloadId,
            Url = faker.Random.AlphaNumeric(11),
            Title = faker.Commerce.ProductName(),
            Summary = faker.Hacker.Phrase(),
            Description = faker.Lorem.Paragraphs(),
            Verification = faker.PickRandom<Verification>(),
            Visibility = faker.PickRandom<Visibility>(),
            Version = faker.System.Version(),
            CreatedAt = faker.Date.Past(),
            UpdatedAt = null
        };
        
        var create = new ICommentService.CreateComment(userId, content);

        await _sut.CreateAsync(download, create);

        var captured = capturedDto.GetLastValue();

        captured.UserId.Should().Be(userId);
        captured.Content.Should().Be(content);
        captured.CreatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
        captured.EditedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnComment_WithMappedData()
    {
        var faker = new Faker();

        var commentId = faker.Random.Guid();
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var content = faker.Lorem.Paragraph();

        A.CallTo(() => _commentRepository.CreateAsync(A<CommentDto>._, A<CancellationToken>._))
            .ReturnsLazily((CommentDto input, CancellationToken _) => new CommentDto
            {
                Id = commentId,
                Download = new DownloadCommentDto { CommentId = commentId, DownloadId = downloadId },
                UserId = input.UserId,
                Content = input.Content,
                CreatedAt = input.CreatedAt,
                EditedAt = input.EditedAt
            });

        var download = new Download
        {
            Id = downloadId,
            Url = faker.Random.AlphaNumeric(11),
            Title = faker.Commerce.ProductName(),
            Summary = faker.Hacker.Phrase(),
            Description = faker.Lorem.Paragraphs(),
            Verification = faker.PickRandom<Verification>(),
            Visibility = faker.PickRandom<Visibility>(),
            Version = faker.System.Version(),
            CreatedAt = faker.Date.Past(),
            UpdatedAt = null
        };
        
        var create = new ICommentService.CreateComment(userId, content);

        var comment = await _sut.CreateAsync(download, create);

        comment.Id.Should().Be(commentId);
        comment.UserId.Should().Be(userId);
        comment.Content.Should().Be(content);
        comment.CreatedAt.Should().BeWithin(TimeSpan.FromSeconds(2));
        comment.EditedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnComment_WhenCommentExists()
    {
        var faker = new Faker();

        var commentId = faker.Random.Guid();
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var content = faker.Lorem.Paragraph();
        var createdAt = faker.Date.Past();
        var editedAt = faker.Date.Recent();

        var commentDto = new CommentDto
        {
            Id = commentId,
            Download = new DownloadCommentDto { CommentId = commentId, DownloadId = downloadId },
            UserId = userId,
            Content = content,
            CreatedAt = createdAt,
            EditedAt = editedAt
        };

        A.CallTo(() => _commentRepository.GetByIdAsync(commentId, A<CancellationToken>._)).Returns(commentDto);

        var comment = await _sut.GetByIdAsync(commentId);

        comment.Should().NotBeNull();

        comment?.Id.Should().Be(commentId);
        comment?.UserId.Should().Be(userId);
        comment?.Content.Should().Be(content);
        comment?.CreatedAt.Should().Be(createdAt);
        comment?.EditedAt.Should().Be(editedAt);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCommentDoesNotExist()
    {
        var commentId = Guid.NewGuid();

        A.CallTo(() => _commentRepository.GetByIdAsync(commentId, A<CancellationToken>._))
            .Returns<CommentDto?>(null);

        var comment = await _sut.GetByIdAsync(commentId);

        comment.Should().BeNull();
    }

    [Fact]
    public async Task GetByDownload_ShouldReturnCollection_WhenCollectionExists()
    {
        var downloadId = Guid.NewGuid();

        var faker = new Faker<CommentDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UserId, f => f.Random.Guid())
            .RuleFor(x => x.Content, f => f.Lorem.Paragraph())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleFor(x => x.EditedAt, f => f.PickRandom(null as DateTime?, f.Date.Past()));

        var collection = faker.Generate(20);

        A.CallTo(() => _commentRepository.GetByDownloadAsync(downloadId, A<CancellationToken>._)).Returns(collection);

        var result = await _sut.GetByDownloadAsync(downloadId);

        foreach (var (i, comment) in result.Select((x, i) => (i, x)))
        {
            comment.Id.Should().Be(collection[i].Id);
            comment.UserId.Should().Be(collection[i].UserId);
            comment.Content.Should().Be(collection[i].Content);
            comment.CreatedAt.Should().Be(collection[i].CreatedAt);
            comment.EditedAt.Should().Be(collection[i].EditedAt);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateComment_WhenCommentExists()
    {
        var faker = new Faker();

        var commentId = faker.Random.Guid();
        var downloadId = faker.Random.Guid();
        var userId = faker.Random.Guid();
        var content = faker.Lorem.Paragraph();
        var createdAt = faker.Date.Past();
        var editedAt = faker.Date.Past();

        var commentDto = new CommentDto
        {
            Id = commentId,
            Download = new DownloadCommentDto { CommentId = commentId, DownloadId = downloadId },
            UserId = userId,
            Content = content,
            CreatedAt = createdAt,
            EditedAt = editedAt
        };

        A.CallTo(() => _commentRepository.GetByIdAsync(commentId, A<CancellationToken>._)).Returns(commentDto);

        var capturedDto = A.Captured<CommentDto>();

        A.CallTo(() =>
            _commentRepository.UpdateAsync(capturedDto.That.Matches(x => x.Id == commentId),
                A<CancellationToken>._)).DoesNothing();

        var updatedContent = faker.Lorem.Paragraph();

        var update = new ICommentService.UpdateComment(updatedContent);

        var result = await _sut.UpdateAsync(commentId, update);

        var captured = capturedDto.GetLastValue();

        captured.Content.Should().Be(updatedContent);
        captured.EditedAt.Should().NotBeNull()
            .And.NotBe(editedAt)
            .And.BeWithin(TimeSpan.FromSeconds(2));

        result.Should().NotBeNull();

        result?.Id.Should().Be(commentId);
        result?.UserId.Should().Be(userId);
        result?.Content.Should().Be(updatedContent);
        result?.EditedAt.Should().Be(captured.EditedAt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryMethod()
    {
        var commentId = Guid.NewGuid();

        await _sut.DeleteAsync(commentId);

        A.CallTo(() => _commentRepository.DeleteAsync(commentId, A<CancellationToken>._)).MustHaveHappened();
    }
}