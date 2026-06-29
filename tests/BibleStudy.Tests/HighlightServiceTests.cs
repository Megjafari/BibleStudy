using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Application.Services;
using BibleStudy.Domain.Entities;
using NSubstitute;

namespace BibleStudy.Tests;

public class HighlightServiceTests
{
    private readonly IHighlightRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly HighlightService _service;
    private const int TestUserId = 1;

    public HighlightServiceTests()
    {
        // Arrange (shared): mocked repository and current user
        _repository = Substitute.For<IHighlightRepository>();
        _currentUser = Substitute.For<ICurrentUser>();
        _currentUser.UserId.Returns(TestUserId);
        _service = new HighlightService(_repository, _currentUser);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenColorIsEmpty()
    {
        // Arrange
        var dto = new CreateHighlightDto("", "John", 3, 16, 16);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenChapterIsZero()
    {
        // Arrange
        var dto = new CreateHighlightDto("yellow", "John", 0, 16, 16);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenEndVerseIsBeforeStartVerse()
    {
        // Arrange
        var dto = new CreateHighlightDto("yellow", "John", 3, 16, 10);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_SetsCurrentUserAsOwner_WhenInputIsValid()
    {
        // Arrange
        var dto = new CreateHighlightDto("yellow", "John", 3, 16, 16);
        _repository.AddAsync(Arg.Any<Highlight>()).Returns(call => call.Arg<Highlight>());

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert: the created highlight is owned by the current user
        await _repository.Received(1).AddAsync(
            Arg.Is<Highlight>(h => h.UserId == TestUserId && h.Book == "John"));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenHighlightDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(99).Returns((Highlight?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenHighlightBelongsToAnotherUser()
    {
        // Arrange: a highlight owned by a different user
        var highlight = new Highlight { Id = 7, UserId = 999, Color = "yellow" };
        _repository.GetByIdAsync(7).Returns(highlight);

        // Act + Assert: the current user must not see it
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(7));
    }
}