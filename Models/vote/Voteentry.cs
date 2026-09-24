namespace WerewolfGM.Web.Models;

/// <summary>
/// Le vote d'un joueur dans une session. Il n'y a qu'une ligne par
/// (session, votant) : un nouveau vote met à jour la cible, ce qui
/// respecte la règle "seul le dernier nom envoyé est pris en compte"
/// pour le vote des loups (et n'a pas d'effet négatif pour le village).
/// </summary>
public class VoteEntry
{
    public int Id { get; set; }

    public int VoteSessionId { get; set; }
    public VoteSession VoteSession { get; set; } = null!;

    public int VoterPlayerId { get; set; }
    public Player VoterPlayer { get; set; } = null!;

    public int TargetPlayerId { get; set; }
    public Player TargetPlayer { get; set; } = null!;

    public DateTime CastAt { get; set; } = DateTime.UtcNow;
}