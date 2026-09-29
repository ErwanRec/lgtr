using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;

namespace WerewolfGM.Web.Models;

// Classe sociale d'un loup (en plus de son rôle)
public enum WolfRank
{
    None = 0,          // pas un loup
    Alpha = 1,         // loup-garou alpha
    Garou = 2,         // loup-garou (dans la meute)
    PresqueGarou = 3   // loup-presque-garou (hors meute au départ)
}

// Une information secrète affichée sur l'accueil d'un joueur
public class PlayerInfo
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // true = générée automatiquement (supprimée à chaque régénération)
    // false = ajoutée à la main par le MJ (jamais supprimée automatiquement)
    public bool IsAuto { get; set; }
}

public static class InfoGenerator
{
    private static readonly string[] SiblingRoles = { "soeurs", "sœurs", "frères", "freres" };

    // Retourne la liste des avertissements (ex: "Aucun Loup Alpha défini")
    public static async Task<List<string>> GenerateAsync(AppDbContext db)
    {
        var warnings = new List<string>();

        var oldAuto = await db.PlayerInfos.Where(i => i.IsAuto).ToListAsync();
        db.PlayerInfos.RemoveRange(oldAuto);

        var players = await db.Players
            .Include(p => p.RoleDefinition)
            .Where(p => !p.IsGameMaster)
            .ToListAsync();

        var alphas = players.Where(p => p.WolfRank == WolfRank.Alpha).ToList();
        var garous = players.Where(p => p.WolfRank == WolfRank.Garou).ToList();
        var almost = players.Where(p => p.WolfRank == WolfRank.PresqueGarou).ToList();
        var others = players.Where(p => p.WolfRank == WolfRank.None).ToList(); // villageois + solos

        if (alphas.Count == 0 && players.Any(p => p.WolfRank != WolfRank.None))
            warnings.Add("Aucun Loup Alpha défini.");

        bool Is(Player p, string part) =>
            p.RoleDefinition?.Name.Contains(part, StringComparison.OrdinalIgnoreCase) == true;

        string Names(IEnumerable<Player> list) => string.Join(", ", list.Select(x => x.FullName));

        Player? Pick(List<Player> list) =>
            list.Count == 0 ? null : list[Random.Shared.Next(list.Count)];

        void Add(Player p, string title, string content) =>
            db.PlayerInfos.Add(new PlayerInfo { PlayerId = p.Id, Title = title, Content = content, IsAuto = true });

        // Deux noms mélangés : un loup + un joueur non-loup (sans dire lequel est lequel)
        void AddPair(Player p, Player? wolf)
        {
            if (wolf is null)
            {
                warnings.Add($"{p.FullName} : aucun loup à lui indiquer.");
                return;
            }

            var names = new[] { wolf.FullName, Pick(others)?.FullName }
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            Add(p, "Deux joueurs à connaître",
                string.Join(" et ", names) + " (l'un des deux est un loup, l'autre non).");
        }

        foreach (var p in players)
        {
            if (p.WolfRank != WolfRank.None)
            {
                // Loup amnésique : aucune info sur la meute
                if (Is(p, "amnésique") || Is(p, "amnesique")) continue;

                // Loup-garou voyant : connaît le loup alpha
                if (Is(p, "voyant"))
                {
                    if (alphas.Count > 0) Add(p, "Loup Alpha", Names(alphas));
                    continue;
                }

                switch (p.WolfRank)
                {
                    case WolfRank.Alpha:
                        Add(p, "Votre meute",
                            garous.Count == 0 ? "Aucun loup-garou dans la meute." : Names(garous));
                        break;

                    case WolfRank.Garou:
                        AddPair(p, Pick(almost));
                        break;

                    case WolfRank.PresqueGarou:
                        AddPair(p, Pick(alphas));
                        break;
                }
            }
            else if (p.RoleDefinition is not null
                     && SiblingRoles.Contains(p.RoleDefinition.Name.Trim().ToLowerInvariant()))
            {
                // Sœurs / frères : ils se connaissent
                var mates = players.Where(x => x.Id != p.Id && x.RoleDefinitionId == p.RoleDefinitionId).ToList();
                if (mates.Count > 0) Add(p, "Vos alliés de confiance", Names(mates));
            }
        }

        await db.SaveChangesAsync();
        return warnings;
    }
}