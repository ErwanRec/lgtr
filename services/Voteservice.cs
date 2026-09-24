using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;
using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Services;

/// <summary>Résultat calculé d'une session de vote, prêt à afficher.</summary>
public class VoteTallyResult
{
    public int SessionId { get; set; }
    public VoteType Type { get; set; }
    public int DayNumber { get; set; }
    public bool IsOpen { get; set; }

    /// <summary>Nombre de votants éligibles (vivants du camp concerné).</summary>
    public int EligibleVoterCount { get; set; }

    /// <summary>Nombre de votants ayant réellement voté.</summary>
    public int VotesCastCount { get; set; }

    /// <summary>Quorum requis atteint (majorité des votants) ? Uniquement pertinent pour le village.</summary>
    public bool QuorumReached { get; set; }

    /// <summary>Total par joueur ciblé (votes + bonus), trié du plus grand au plus petit.</summary>
    public List<(Player Target, int RawVotes, int BonusVotes, int Total)> Standings { get; set; } = new();

    /// <summary>Joueur en tête, s'il n'y a pas d'égalité.</summary>
    public Player? Winner { get; set; }

    public bool IsTie { get; set; }
}

public class VoteService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public VoteService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<VoteSession> OpenSessionAsync(VoteType type, int dayNumber)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var session = new VoteSession { Type = type, DayNumber = dayNumber, IsOpen = true };
        db.VoteSessions.Add(session);
        await db.SaveChangesAsync();
        return session;
    }

    public async Task<VoteSession?> GetActiveSessionAsync(VoteType type)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.VoteSessions
            .Where(s => s.Type == type && s.IsOpen)
            .OrderByDescending(s => s.OpenedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<VoteSession>> GetAllSessionsAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.VoteSessions.OrderByDescending(s => s.OpenedAt).ToListAsync();
    }

    /// <summary>
    /// Enregistre (ou met à jour) le vote d'un joueur. Comme il n'y a qu'une
    /// seule ligne par (session, votant), un nouveau vote écrase le
    /// précédent : c'est exactement la règle "seul le dernier nom envoyé
    /// compte" pour le vote des loups, et ça ne pose pas de souci pour le
    /// village (les joueurs changent rarement d'avis, mais s'ils le font,
    /// c'est aussi le comportement attendu).
    /// </summary>
    public async Task CastVoteAsync(int sessionId, int voterPlayerId, int targetPlayerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var session = await db.VoteSessions.FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new InvalidOperationException("Session de vote introuvable.");
        if (!session.IsOpen)
            throw new InvalidOperationException("Cette session de vote est fermée.");

        var existing = await db.VoteEntries
            .FirstOrDefaultAsync(v => v.VoteSessionId == sessionId && v.VoterPlayerId == voterPlayerId);

        if (existing is null)
        {
            db.VoteEntries.Add(new VoteEntry
            {
                VoteSessionId = sessionId,
                VoterPlayerId = voterPlayerId,
                TargetPlayerId = targetPlayerId
            });
        }
        else
        {
            existing.TargetPlayerId = targetPlayerId;
            existing.CastAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public async Task AddBonusAsync(int sessionId, int targetPlayerId, int amount, string reason)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.VoteBonuses.Add(new VoteBonus
        {
            VoteSessionId = sessionId,
            TargetPlayerId = targetPlayerId,
            Amount = amount,
            Reason = reason
        });
        await db.SaveChangesAsync();
    }

    public async Task CloseSessionAsync(int sessionId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var session = await db.VoteSessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null) return;
        session.IsOpen = false;
        session.ClosedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task ReopenSessionAsync(int sessionId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var session = await db.VoteSessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null) return;
        session.IsOpen = true;
        session.ClosedAt = null;
        await db.SaveChangesAsync();
    }

    public async Task<Player?> GetMyVoteAsync(int sessionId, int voterPlayerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var entry = await db.VoteEntries
            .Include(v => v.TargetPlayer)
            .FirstOrDefaultAsync(v => v.VoteSessionId == sessionId && v.VoterPlayerId == voterPlayerId);
        return entry?.TargetPlayer;
    }

    public async Task<VoteSession?> GetSessionByIdAsync(int sessionId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.VoteSessions.FirstOrDefaultAsync(s => s.Id == sessionId);
    }
    /// <summary>
    /// Calcule le dépouillement en direct : quorum, total par cible (votes +
    /// bonus manuels du MJ), et le joueur en tête (ou l'égalité).
    /// </summary>
    public async Task<VoteTallyResult> GetTallyAsync(int sessionId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var session = await db.VoteSessions
            .Include(s => s.Entries).ThenInclude(e => e.TargetPlayer)
            .Include(s => s.Entries).ThenInclude(e => e.VoterPlayer)
            .Include(s => s.Bonuses).ThenInclude(b => b.TargetPlayer)
            .FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new InvalidOperationException("Session de vote introuvable.");

        // Votants éligibles : joueurs vivants, et pour le vote des loups
        // uniquement ceux du camp Loups (la meute).
        var eligibleQuery = db.Players.Where(p => !p.IsGameMaster && p.IsAlive);
        if (session.Type == VoteType.Loups)
            eligibleQuery = eligibleQuery.Where(p => p.Camp == Camp.Loups);
        var eligibleCount = await eligibleQuery.CountAsync();

        var castCount = session.Entries.Select(e => e.VoterPlayerId).Distinct().Count();

        var rawCounts = session.Entries
            .GroupBy(e => e.TargetPlayer)
            .ToDictionary(g => g.Key, g => g.Count());

        var bonusCounts = session.Bonuses
            .GroupBy(b => b.TargetPlayer)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));

        var allTargets = rawCounts.Keys.Union(bonusCounts.Keys, PlayerIdComparer.Instance);

        var standings = allTargets
            .Select(p =>
            {
                var raw = rawCounts.TryGetValue(p, out var r) ? r : 0;
                var bonus = bonusCounts.TryGetValue(p, out var b) ? b : 0;
                return (Target: p, RawVotes: raw, BonusVotes: bonus, Total: raw + bonus);
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        Player? winner = null;
        bool isTie = false;
        if (standings.Count > 0)
        {
            var top = standings[0].Total;
            var leaders = standings.Where(s => s.Total == top).ToList();
            if (leaders.Count == 1) winner = leaders[0].Target;
            else isTie = true;
        }

        // Quorum : uniquement une notion pour le vote du village d'après les règles
        // ("il faut qu'il y ait une majorité de votants pour que le vote soit comptabilisé").
        bool quorumReached = session.Type != VoteType.Village
            || eligibleCount == 0
            || castCount > eligibleCount / 2.0;

        return new VoteTallyResult
        {
            SessionId = session.Id,
            Type = session.Type,
            DayNumber = session.DayNumber,
            IsOpen = session.IsOpen,
            EligibleVoterCount = eligibleCount,
            VotesCastCount = castCount,
            QuorumReached = quorumReached,
            Standings = standings,
            Winner = winner,
            IsTie = isTie
        };
    }

    private class PlayerIdComparer : IEqualityComparer<Player>
    {
        public static readonly PlayerIdComparer Instance = new();
        public bool Equals(Player? x, Player? y) => x?.Id == y?.Id;
        public int GetHashCode(Player obj) => obj.Id;
    }
}