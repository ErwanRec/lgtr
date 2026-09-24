namespace WerewolfGM.Web.Models;

/// <summary>
/// Un salon de discussion (équivalent d'un groupe WhatsApp) : "Village",
/// "Loups", "Morts", "Killers", ou tout groupe ad hoc créé par le MJ.
/// </summary>
public class ChatGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Icône/emoji affiché dans la liste des salons.</summary>
    public string Emoji { get; set; } = "💬";

    public bool IsSystemGroup { get; set; } // ex: groupe "Village" créé automatiquement
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ChatGroupMember> Members { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
}