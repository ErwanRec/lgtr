namespace WerewolfGM.Web.Models;

public class ChatGroupMember
{
    public int Id { get; set; }

    public int ChatGroupId { get; set; }
    public ChatGroup ChatGroup { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}