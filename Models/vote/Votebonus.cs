namespace WerewolfGM.Web.Models;

/// <summary>
/// Votes supplémentaires accordés manuellement par le MJ à un joueur
/// pour une session donnée (corbeau, ange, vote des morts en cas
/// d'égalité, etc.).
/// </summary>
public class VoteBonus
{
    public int Id { get; set; }

    public int VoteSessionId { get; set; }
    public VoteSession VoteSession { get; set; } = null!;

    public int TargetPlayerId { get; set; }
    public Player TargetPlayer { get; set; } = null!;

    public int Amount { get; set; } = 1;
    public string Reason { get; set; } = string.Empty;
}