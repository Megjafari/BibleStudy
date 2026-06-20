using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Application.Services;
using BibleStudy.Domain.Entities;
using NSubstitute;

namespace BibleStudy.Tests;

public class PlanServiceTests
{
    private readonly IPlanRepository _repository;
    private readonly PlanService _service;

    public PlanServiceTests()
    {
        // Arrange (shared): a mocked repository injected into the service
        _repository = Substitute.For<IPlanRepository>();
        _service = new PlanService(_repository);
    }

    [Fact]
    public async Task GetProgressAsync_ReturnsCorrectPercentage_WhenSomeReadingsAreRead()
    {
        // Arrange
        var plan = new Plan
        {
            Id = 1,
            Readings = new List<Reading>
            {
                new() { IsRead = true },
                new() { IsRead = true },
                new() { IsRead = false },
                new() { IsRead = false }
            }
        };
        _repository.GetByIdWithReadingsAsync(1).Returns(plan);

        // Act
        var result = await _service.GetProgressAsync(1);

        // Assert
        Assert.Equal(4, result.TotalReadings);
        Assert.Equal(2, result.ReadReadings);
        Assert.Equal(50, result.PercentComplete);
    }

    [Fact]
    public async Task GetProgressAsync_ReturnsZero_WhenPlanHasNoReadings()
    {
        // Arrange
        var plan = new Plan { Id = 1, Readings = new List<Reading>() };
        _repository.GetByIdWithReadingsAsync(1).Returns(plan);

        // Act
        var result = await _service.GetProgressAsync(1);

        // Assert
        Assert.Equal(0, result.TotalReadings);
        Assert.Equal(0, result.PercentComplete);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenPlanDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(99).Returns((Plan?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = new CreatePlanDto("", null, DateTime.UtcNow);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_SavesPlan_WhenTitleIsValid()
    {
        // Arrange
        var dto = new CreatePlanDto("The Gospel of John", null, DateTime.UtcNow);
        _repository.AddAsync(Arg.Any<Plan>()).Returns(call => call.Arg<Plan>());

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("The Gospel of John", result.Title);
        await _repository.Received(1).AddAsync(Arg.Any<Plan>());
    }
}