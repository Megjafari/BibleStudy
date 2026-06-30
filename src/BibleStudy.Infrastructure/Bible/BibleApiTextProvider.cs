using System.Text.Json;
using BibleStudy.Application.DTOs;
using BibleStudy.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BibleStudy.Infrastructure.Bible;

public class BibleApiTextProvider : IBibleTextProvider
{
    private readonly HttpClient _httpClient;

    // Maps a user-facing translation code to its API.Bible bible ID.
    private static readonly Dictionary<string, string> Translations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kjv"] = "de4e12af7f28f599-01",
        ["skb"] = "fa4317c59f0825e0-01"
    };

    public BibleApiTextProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.scripture.api.bible/");
        _httpClient.DefaultRequestHeaders.Add("api-key", configuration["BibleApi:ApiKey"]);
    }

    public async Task<BibleChapterDto> GetChapterAsync(string book, int chapter, string translation)
    {
        if (!Translations.TryGetValue(translation, out var bibleId))
            throw new ArgumentException($"Unknown translation '{translation}'.");

        var url = $"v1/bibles/{bibleId}/chapters/{book}.{chapter}" +
                  "?content-type=json&include-verse-numbers=true";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Failed to fetch {book} {chapter} ({translation}): {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();
        return ParseChapter(json, book, chapter, translation);
    }

    public async Task<IEnumerable<BibleBookDto>> GetBooksAsync(string translation)
    {
        if (!Translations.TryGetValue(translation, out var bibleId))
            throw new ArgumentException($"Unknown translation '{translation}'.");

        var response = await _httpClient.GetAsync($"v1/bibles/{bibleId}/books");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Failed to fetch books ({translation}): {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();
        return ParseBooks(json);
    }

    public async Task<IEnumerable<int>> GetChaptersAsync(string book, string translation)
    {
        if (!Translations.TryGetValue(translation, out var bibleId))
            throw new ArgumentException($"Unknown translation '{translation}'.");

        var response = await _httpClient.GetAsync($"v1/bibles/{bibleId}/books/{book}/chapters");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Failed to fetch chapters for {book} ({translation}): {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();
        return ParseChapters(json);
    }

    private static IEnumerable<BibleBookDto> ParseBooks(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement.GetProperty("data");

        var books = new List<BibleBookDto>();
        var reachedNewTestament = false;

        foreach (var item in data.EnumerateArray())
        {
            var id = item.GetProperty("id").GetString()!;
            var name = item.GetProperty("name").GetString() ?? id;

            // Everything from Matthew onward is the New Testament.
            if (id == "MAT")
                reachedNewTestament = true;

            books.Add(new BibleBookDto(id, name, reachedNewTestament ? "NT" : "OT"));
        }

        return books;
    }

    private static IEnumerable<int> ParseChapters(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement.GetProperty("data");

        var chapters = new List<int>();
        foreach (var item in data.EnumerateArray())
        {
            var number = item.GetProperty("number").GetString();
            // API.Bible includes an "intro" chapter; skip non-numeric.
            if (int.TryParse(number, out var n))
                chapters.Add(n);
        }

        return chapters;
    }

    private static BibleChapterDto ParseChapter(string json, string book, int chapter, string translation)
{   
    using var doc = JsonDocument.Parse(json);
    var data = doc.RootElement.GetProperty("data");

    var reference = data.GetProperty("reference").GetString() ?? $"{book} {chapter}";
    var copyright = data.TryGetProperty("copyright", out var c) ? c.GetString() ?? "" : "";

    var verses = new List<BibleVerseDto>();
    var currentVerse = 0;
    var currentText = new System.Text.StringBuilder();

    void Walk(JsonElement element)
    {
        var type = element.GetProperty("type").GetString();
        var name = element.TryGetProperty("name", out var n) ? n.GetString() : null;

        if (type == "tag" && name == "verse")
        {
            // A new verse marker: store the verse we were building.
            if (currentVerse > 0)
                verses.Add(new BibleVerseDto(currentVerse, currentText.ToString().Trim()));

            currentVerse = int.Parse(element.GetProperty("attrs").GetProperty("number").GetString()!);
            currentText.Clear();
            return;
        }

        if (type == "text" && currentVerse > 0)
        {
            // Any text belonging to the current verse, including quoted speech.
            currentText.Append(element.GetProperty("text").GetString());
        }

        // Recurse into nested items (verses can contain sub-paragraphs).
        if (element.TryGetProperty("items", out var items))
        {
            foreach (var child in items.EnumerateArray())
                Walk(child);
        }
    }

    foreach (var para in data.GetProperty("content").EnumerateArray())
        Walk(para);

    // Add the final verse once traversal ends.
    if (currentVerse > 0)
        verses.Add(new BibleVerseDto(currentVerse, currentText.ToString().Trim()));

    return new BibleChapterDto(reference, book, chapter, translation, copyright, verses);
}
}