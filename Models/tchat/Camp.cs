namespace WerewolfGM.Web.Models;

/// <summary>
/// Camp (équipe) auquel appartient un joueur. Peut changer en cours de partie
/// (infection, enfant sauvage qui se transforme, chien-loup, etc.), donc c'est
/// stocké sur le joueur et pas uniquement déduit du rôle de départ.
/// </summary>
public enum Camp
{
    Villageois,
    Loups,
    Solo,
    Couple
}