namespace BibleStudy.Domain.Entities;

public class Reading
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public string Book { get; set; } = string.Empty;
    public int StartChapter { get; set; }
    public int EndChapter { get; set; }
    public int DayNumber { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    public Plan Plan { get; set; } = null!;
}