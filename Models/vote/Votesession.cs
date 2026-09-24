namespace WerewolfGM.Web.Models;

/// <summary>
/// Une session de vote (le vote du village du jour 3, le vote des loups du
/// jour 5, etc.). Le MJ l'ouvre, les joueurs concernés votent, le MJ la
/// referme pour figer le résultat.
/// </summary>
public class VoteSession
{
    public int Id { get; set; }
    public VoteType Type { get; set; }
    public int DayNumber { get; set; }

    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// Bonus de vote manuel ajouté par le MJ pour un joueur cible (ex: vote
    /// des morts qui ajoute +1 en cas d'égalité, corbeau, ange, etc.).
    /// Clé = PlayerId de la cible, valeur = nombre de votes bonus.
    /// </summary>
    public List<VoteBonus> Bonuses { get; set; } = new();

    public List<VoteEntry> Entries { get; set; } = new();
}