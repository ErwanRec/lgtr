namespace WerewolfGM.Web.Models;

public enum VoteType
{
    /// <summary>Vote du village à 13h30 : tout joueur vivant peut voter.</summary>
    Village,

    /// <summary>Vote des loups à 22h : seuls les loups de la meute votent, un seul mort max (sauf grand méchant loup).</summary>
    Loups
}