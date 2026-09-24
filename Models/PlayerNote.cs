namespace WerewolfGM.Web.Models;

public class PlayerNote
{
    public int Id { get; set; }

    public int AuthorPlayerId { get; set; }
    public Player Author { get; set; } = null!;

    public int TargetPlayerId { get; set; }
    public Player Target { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}