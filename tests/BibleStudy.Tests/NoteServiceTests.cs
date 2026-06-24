using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Application.Services;
using BibleStudy.Domain.Entities;
using NSubstitute;

namespace BibleStudy.Tests;

public class NoteServiceTests
{
    private readonly INoteRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly NoteService _service;
    private const int TestUserId = 1;

    public NoteServiceTests()
    {
        // Arrange (shared): mocked repository and current user
        _repository = Substitute.For<INoteRepository>();
        _currentUser = Substitute.For<ICurrentUser>();
        _currentUser.UserId.Returns(TestUserId);
        _service = new NoteService(_repository, _currentUser);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenContentIsEmpty()
    {
        // Arrange
        var dto = new CreateNoteDto("", "John", 3, 16, 16);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenChapterIsZero()
    {
        // Arrange
        var dto = new CreateNoteDto("A note", "John", 0, 16, 16);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenEndVerseIsBeforeStartVerse()
    {
        // Arrange
        var dto = new CreateNoteDto("A note", "John", 3, 16, 10);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_SetsCurrentUserAsOwner_WhenInputIsValid()
    {
        // Arrange
        var dto = new CreateNoteDto("For God so loved the world", "John", 3, 16, 16);
        _repository.AddAsync(Arg.Any<Note>()).Returns(call => call.Arg<Note>());

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert: the created note is owned by the current user
        await _repository.Received(1).AddAsync(
            Arg.Is<Note>(n => n.UserId == TestUserId && n.Book == "John"));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(99).Returns((Note?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenNoteBelongsToAnotherUser()
    {
        // Arrange: a note owned by a different user
        var note = new Note { Id = 7, UserId = 999, Content = "Someone else's note" };
        _repository.GetByIdAsync(7).Returns(note);

        // Act + Assert: the current user must not see it
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(7));
    }
}