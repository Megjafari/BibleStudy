namespace BibleStudy.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Plan> Plans { get; set; } = new List<Plan>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}