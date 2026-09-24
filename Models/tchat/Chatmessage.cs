namespace WerewolfGM.Web.Models;

public class ChatMessage
{
    public int Id { get; set; }

    public int ChatGroupId { get; set; }
    public ChatGroup ChatGroup { get; set; } = null!;

    /// <summary>Null = message système (annonce du MJ, mort, résultat de vote...).</summary>
    public int? SenderPlayerId { get; set; }
    public Player? SenderPlayer { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsSystemMessage { get; set; }
}