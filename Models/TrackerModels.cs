using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;

namespace WerewolfGM.Web.Models;

// Un champ de suivi lié au rôle d'un joueur (ex: "Pouvoirs disponibles : 3")
public class TrackerField
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Order { get; set; }
}

// Un ciblage : SourcePlayer (un loup ou le chasseur) a ciblé TargetPlayer
public class PlayerTarget
{
    public int Id { get; set; }
    public int SourcePlayerId { get; set; }
    public Player? SourcePlayer { get; set; }
    public int TargetPlayerId { get; set; }
    public Player? TargetPlayer { get; set; }
}

public static class TrackerGenerator
{
    private static (Func<string, bool> Match, (string Label, string Default)[] Fields) T(
        Func<string, bool> match, params (string, string)[] fields) => (match, fields);

    // Champs par rôle, d'après la feuille de suivi (le nom du rôle est normalisé : sans accents ni tirets)
    private static readonly (Func<string, bool> Match, (string Label, string Default)[] Fields)[] Templates =
    {
        T(n => n.Contains("blanc"),                          ("Kills supplémentaires", "1")),
        T(n => n.Contains("infect"),                         ("Infection disponible", "Oui")),
        T(n => n.Contains("loup") && n.Contains("voyant"),   ("Pouvoirs disponibles", "3")),
        T(n => n.Contains("assassin"),                       ("Objet du jour", "")),
        T(n => n.Contains("poete"),                          ("Phrase dite", "")),
        T(n => n.StartsWith("ange"),                         ("Type", "Protecteur"), ("Cible", ""), ("Cible en vie", "Oui"), ("Votes disponibles", "1")),
        T(n => n.Contains("corbeau"),                        ("Votes disponibles", "5")),
        T(n => n.Contains("hypnotiseur"),                    ("Cible", ""), ("Hypnotisé", "")),
        T(n => n.Contains("boulanger"),                      ("Cible", ""), ("Heure", "")),
        T(n => n.Contains("servante"),                       ("Rôle récupéré", "")),
        T(n => n.Contains("voleur"),                         ("Rôle récupéré", "")),
        T(n => n == "ancien",                                ("Vies bonus", "1")),
        T(n => n.Contains("pot de colle"),                   ("Cible", "")),
        T(n => n.Contains("enfant sauvage"),                 ("Modèle", ""), ("Modèle en vie", "Oui")),
        T(n => n.Contains("idiot"),                          ("Rôle révélé", "Non")),
        T(n => n.Contains("rival"),                          ("Votes", "1")),
        T(n => n.Contains("cupidon"),                        ("Amoureux", "")),
        T(n => n.Contains("squatt"),                         ("Cible", "")),
        T(n => n.Contains("salvateur"),                      ("Joueur protégé", "")),
        T(n => n.Contains("chien"),                          ("Choix", "Simple villageois")),
        T(n => n.Contains("sorciere"),                       ("Potions restantes", "3")),
        T(n => n.Contains("voyante"),                        ("Pouvoirs disponibles", "3")),
        T(n => n == "juge",                                  ("Pouvoirs disponibles", "3")),
        T(n => n.Contains("petite fille"),                   ("Heure", "")),
        T(n => n.Contains("flute"),                          ("Joueurs enchantés", "")),
    };

    private static void AddFields(AppDbContext db, Player player)
    {
        var name = InfoGenerator.Norm(player.RoleDefinition?.Name);
        var template = Templates.FirstOrDefault(t => t.Match(name));
        if (template.Fields is null) return;

        var order = 0;
        foreach (var (label, def) in template.Fields)
        {
            db.TrackerFields.Add(new TrackerField
            {
                PlayerId = player.Id, Label = label, Value = def, Order = order++
            });
        }
    }

    // Recrée les champs de suivi d'un seul joueur (après un changement de rôle)
    public static async Task ResetPlayerAsync(AppDbContext db, int playerId)
    {
        var player = await db.Players.Include(p => p.RoleDefinition).FirstOrDefaultAsync(p => p.Id == playerId);
        if (player is null) return;

        db.TrackerFields.RemoveRange(await db.TrackerFields.Where(f => f.PlayerId == playerId).ToListAsync());
        AddFields(db, player);
        await db.SaveChangesAsync();
    }

    // Recrée les champs de suivi de tous les joueurs (nouvelle partie)
    public static async Task ResetAllAsync(AppDbContext db)
    {
        db.TrackerFields.RemoveRange(await db.TrackerFields.ToListAsync());

        var players = await db.Players.Include(p => p.RoleDefinition).Where(p => !p.IsGameMaster).ToListAsync();
        foreach (var p in players) AddFields(db, p);

        await db.SaveChangesAsync();
    }
}
