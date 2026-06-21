using BibleStudy.Application.DTOs;
using BibleStudy.Application.Interfaces;
using NSubstitute;

namespace BibleStudy.Tests;

public class BibleTextProviderTests
{
    [Fact]
    public async Task GetChapterAsync_ReturnsVerses_WhenProviderSucceeds()
    {
        // Arrange
        var provider = Substitute.For<IBibleTextProvider>();
        var expected = new BibleChapterDto(
            "John 3", "JHN", 3, "kjv", "PUBLIC DOMAIN",
            new List<BibleVerseDto>
            {
                new(1, "There was a man of the Pharisees, named Nicodemus..."),
                new(2, "The same came to Jesus by night...")
            });
        provider.GetChapterAsync("JHN", 3, "kjv").Returns(expected);

        // Act
        var result = await provider.GetChapterAsync("JHN", 3, "kjv");

        // Assert
        Assert.Equal("John 3", result.Reference);
        Assert.Equal(2, result.Verses.Count());
        Assert.Equal(1, result.Verses.First().Verse);
    }

    [Fact]
    public async Task GetChapterAsync_Throws_WhenProviderFails()
    {
        // Arrange
        var provider = Substitute.For<IBibleTextProvider>();
        provider.GetChapterAsync("JHN", 3, "kjv")
            .Returns<Task<BibleChapterDto>>(_ =>
                throw new HttpRequestException("External API failed."));

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => provider.GetChapterAsync("JHN", 3, "kjv"));
    }
}