namespace BibleStudy.Domain.Entities;

public class Highlight
{
    public int Id { get; set; }
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int StartVerse { get; set; }
    public int EndVerse { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}