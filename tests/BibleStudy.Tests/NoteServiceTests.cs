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
    private readonly NoteService _service;

    public NoteServiceTests()
    {
        // Arrange (shared): a mocked repository injected into the service
        _repository = Substitute.For<INoteRepository>();
        _service = new NoteService(_repository);
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
    public async Task CreateAsync_SavesNote_WhenInputIsValid()
    {
        // Arrange
        var dto = new CreateNoteDto("For God so loved the world", "John", 3, 16, 16);
        _repository.AddAsync(Arg.Any<Note>()).Returns(call => call.Arg<Note>());

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("John", result.Book);
        Assert.Equal(16, result.StartVerse);
        await _repository.Received(1).AddAsync(Arg.Any<Note>());
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
}