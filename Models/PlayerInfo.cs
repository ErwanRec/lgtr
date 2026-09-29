using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;

namespace WerewolfGM.Web.Models;

// Une information secrète affichée sur l'accueil d'un joueur
public class PlayerInfo
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // true = générée automatiquement, false = ajoutée à la main par le MJ
    public bool IsAuto { get; set; }
}

public static class InfoGenerator
{
    private enum Kind { None, Alpha, Presque, Voyant, PairWolf, Sister, Brother }

    // "Loup-garou Voyant" -> "loup garou voyant" ; "Infect père des loups" -> "infect pere des loups"
    private static string Norm(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var sb = new StringBuilder();
        foreach (var c in s.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);

        var t = sb.ToString().ToLowerInvariant()
            .Replace("œ", "oe").Replace('-', ' ').Replace('\'', ' ').Replace('’', ' ');
        return string.Join(' ', t.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static bool IsClassicWolf(string n) => n is "loup garou" or "simple loup garou";

    private static Kind Classify(string n)
    {
        if (n.Contains("presque")) return Kind.Presque;
        if (n.Contains("alpha")) return Kind.Alpha;
        if (n.Contains("loup") && n.Contains("voyant")) return Kind.Voyant;

        if (IsClassicWolf(n)
            || n.Contains("infect")
            || n.Contains("poete")
            || n.Contains("feutre")
            || n.Contains("grand mechant"))
            return Kind.PairWolf;

        if (n.StartsWith("soeur")) return Kind.Sister;
        if (n.StartsWith("frere")) return Kind.Brother;
        return Kind.None;
    }

    public static async Task<List<string>> GenerateAsync(AppDbContext db)
    {
        var warnings = new List<string>();

        db.PlayerInfos.RemoveRange(await db.PlayerInfos.Where(i => i.IsAuto).ToListAsync());

        var players = await db.Players
            .Include(p => p.RoleDefinition)
            .Where(p => !p.IsGameMaster)
            .OrderBy(p => p.Id)
            .ToListAsync();

        var entries = players
            .Select(p => (Player: p, Name: Norm(p.RoleDefinition?.Name)))
            .Select(x => (x.Player, x.Name, Kind: Classify(x.Name)))
            .ToList();

        var alphas   = entries.Where(e => e.Kind == Kind.Alpha).Select(e => e.Player).ToList();
        var presques = entries.Where(e => e.Kind == Kind.Presque).Select(e => e.Player).ToList();
        var alphaKnown = entries
            .Where(e => IsClassicWolf(e.Name) || e.Kind == Kind.Voyant)
            .Select(e => e.Player).ToList();
        var nonWolves = players.Where(p => p.Camp != Camp.Loups).ToList();

        string Names(IEnumerable<Player> l) => string.Join(", ", l.Select(x => x.FullName));
        Player? Pick(List<Player> l) => l.Count == 0 ? null : l[Random.Shared.Next(l.Count)];

        void Add(Player p, string title, string content) =>
            db.PlayerInfos.Add(new PlayerInfo { PlayerId = p.Id, Title = title, Content = content, IsAuto = true });

        // Deux noms mélangés : un loup + un joueur hors du clan des loups
        void AddPair(Player p, Player? wolf, string wolfLabel)
        {
            if (wolf is null)
            {
                warnings.Add($"{p.FullName} : aucun {wolfLabel} dans la partie.");
                return;
            }

            var names = new[] { wolf.FullName, Pick(nonWolves.Where(x => x.Id != p.Id).ToList())?.FullName }
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            Add(p, "Deux joueurs à connaître",
                string.Join(" et ", names) + " (l'un des deux est un loup, l'autre non).");
        }

        foreach (var (p, _, kind) in entries)
        {
            switch (kind)
            {
                case Kind.Alpha:
                    Add(p, "Votre meute",
                        alphaKnown.Count == 0 ? "Aucun loup pour l'instant." : Names(alphaKnown));
                    break;

                case Kind.Voyant:
                    if (alphas.Count == 0) warnings.Add($"{p.FullName} : aucun Loup Alpha dans la partie.");
                    else Add(p, "Loup Alpha", Names(alphas));
                    break;

                case Kind.PairWolf:
                    AddPair(p, Pick(presques), "loup presque-garou");
                    break;

                case Kind.Presque:
                    AddPair(p, Pick(alphas), "loup alpha");
                    break;

                case Kind.Sister:
                {
                    var others = entries.Where(e => e.Kind == Kind.Sister
                        && e.Player.RoleDefinitionId == p.RoleDefinitionId && e.Player.Id != p.Id)
                        .Select(e => e.Player).ToList();
                    if (others.Count > 0) Add(p, "Votre sœur", Names(others));
                    break;
                }

                case Kind.Brother:
                {
                    // Triangle : chaque frère connaît le suivant (A→B, B→C, C→A)
                    var group = entries.Where(e => e.Kind == Kind.Brother
                        && e.Player.RoleDefinitionId == p.RoleDefinitionId)
                        .Select(e => e.Player).OrderBy(x => x.Id).ToList();
                    if (group.Count > 1)
                    {
                        var next = group[(group.IndexOf(p) + 1) % group.Count];
                        Add(p, "Un de vos frères", next.FullName);
                    }
                    break;
                }
            }
        }

        await db.SaveChangesAsync();
        return warnings;
    }
}