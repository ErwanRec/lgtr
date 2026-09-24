using System.ComponentModel.DataAnnotations;

namespace WerewolfGM.Web.Models;

/// <summary>
/// Un participant : soit un joueur, soit un MJ (IsGameMaster = true).
/// La connexion se fait avec un code personnel (le "numéro à 3 chiffres"
/// évoqué dans les règles), pas avec un mot de passe classique.
/// </summary>
public class Player
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Code de connexion unique (3 chiffres par défaut, personnalisable).</summary>
    [Required, MaxLength(20)]
    public string AccessCode { get; set; } = string.Empty;

    public bool IsGameMaster { get; set; }

    public int? RoleDefinitionId { get; set; }
    public RoleDefinition? RoleDefinition { get; set; }

    public Camp Camp { get; set; } = Camp.Villageois;

    public bool IsAlive { get; set; } = true;

    /// <summary>A-t-il le pouvoir "killer" en plus de son rôle ?</summary>
    public bool HasKillerPower { get; set; }

    /// <summary>
    /// Notes libres du MJ pour ce joueur : pouvoirs utilisés, cibles, potions
    /// restantes, protections en cours, etc. (équivalent des colonnes libres
    /// du fichier Excel "Suivi de la partie").
    /// </summary>
    [MaxLength(4000)]
    public string McNotes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}";
}