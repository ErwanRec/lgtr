namespace WerewolfGM.Web.Models;

/// <summary>
/// Fiche d'un rôle du jeu (catalogue). C'est la "carte" affichée dans la
/// bibliothèque de rôles, indépendante des joueurs qui l'incarnent.
/// </summary>
public class RoleDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Camp Camp { get; set; }
    public string Emoji { get; set; } = "🃏";

    /// <summary>Résumé affiché sur la carte (une phrase).</summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>Description complète des pouvoirs / conditions de victoire.</summary>
    public string FullDescription { get; set; } = string.Empty;

    /// <summary>Est-ce un rôle qui a une action à heure fixe (ex: sorcière, boulanger) ?</summary>
    public bool HasFixedTimeAction { get; set; }
}