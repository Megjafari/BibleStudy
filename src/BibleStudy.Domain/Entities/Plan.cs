namespace BibleStudy.Domain.Entities;

public class Plan
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
}