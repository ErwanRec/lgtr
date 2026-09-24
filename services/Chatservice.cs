using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;
using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Services;

/// <summary>
/// Gère les salons de discussion (équivalent des groupes WhatsApp des règles) :
/// - "Village" : salon système commun, ouvert à tout joueur vivant sans
///   adhésion explicite (annonce des morts, vote du village).
/// - Meute des loups, "Morts", ou tout salon ad hoc : nécessitent une
///   adhésion explicite via ChatGroupMember (ajout par le MJ ou le loup alpha).
/// Le MJ a toujours accès à tous les salons, pour la modération.
/// </summary>
public class ChatService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ChatService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // ---------- Salons ----------

    public async Task<List<ChatGroup>> GetAllGroupsAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ChatGroups
            .OrderByDescending(g => g.IsSystemGroup)
            .ThenBy(g => g.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Salons visibles par un joueur : le MJ voit tout ; un joueur voit le
    /// salon "Village" tant qu'il est vivant, plus les salons où il a une
    /// adhésion explicite (meute des loups, salon des morts, salons ad hoc).
    /// </summary>
    public async Task<List<ChatGroup>> GetVisibleGroupsAsync(int playerId, bool isGameMaster)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        if (isGameMaster)
        {
            return await db.ChatGroups
                .OrderByDescending(g => g.IsSystemGroup)
                .ThenBy(g => g.Name)
                .ToListAsync();
        }

        var player = await db.Players.FirstOrDefaultAsync(p => p.Id == playerId);
        if (player is null) return new List<ChatGroup>();

        var memberGroupIds = await db.ChatGroupMembers
            .Where(m => m.PlayerId == playerId)
            .Select(m => m.ChatGroupId)
            .ToListAsync();

        return await db.ChatGroups
            .Where(g => memberGroupIds.Contains(g.Id)
                     || (g.IsSystemGroup && g.Name == "Village" && player.IsAlive))
            .OrderByDescending(g => g.IsSystemGroup)
            .ThenBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<ChatGroup?> GetGroupByIdAsync(int chatGroupId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ChatGroups.FirstOrDefaultAsync(g => g.Id == chatGroupId);
    }

    /// <summary>Récupère un salon système par son nom, ou le crée s'il n'existe pas encore (ex: "Morts").</summary>
    public async Task<ChatGroup> GetOrCreateSystemGroupAsync(string name, string emoji)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var group = await db.ChatGroups.FirstOrDefaultAsync(g => g.Name == name);
        if (group is not null) return group;

        group = new ChatGroup { Name = name, Emoji = emoji, IsSystemGroup = true };
        db.ChatGroups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }

    /// <summary>Crée un salon (ex: la meute des loups à la demande du loup alpha, ou un salon MJ ad hoc).</summary>
    public async Task<ChatGroup> CreateGroupAsync(string name, string emoji, bool isSystemGroup = false)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var group = new ChatGroup { Name = name.Trim(), Emoji = emoji, IsSystemGroup = isSystemGroup };
        db.ChatGroups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }

    // ---------- Membres ----------

    /// <summary>
    /// Un joueur fait-il partie de ce salon ? Le MJ a toujours accès. Le salon
    /// "Village" est ouvert à tout joueur vivant sans adhésion explicite. Les
    /// autres salons (meute, morts, ad hoc) nécessitent une ligne dans
    /// ChatGroupMembers.
    /// </summary>
    public async Task<bool> IsMemberAsync(int chatGroupId, int playerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var player = await db.Players.FirstOrDefaultAsync(p => p.Id == playerId);
        if (player is null) return false;
        if (player.IsGameMaster) return true;

        var group = await db.ChatGroups.FirstOrDefaultAsync(g => g.Id == chatGroupId);
        if (group is null) return false;

        if (group.IsSystemGroup && group.Name == "Village")
            return player.IsAlive;

        return await db.ChatGroupMembers
            .AnyAsync(m => m.ChatGroupId == chatGroupId && m.PlayerId == playerId);
    }

    public async Task<List<Player>> GetMembersAsync(int chatGroupId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ChatGroupMembers
            .Where(m => m.ChatGroupId == chatGroupId)
            .Select(m => m.Player)
            .OrderBy(p => p.LastName)
            .ToListAsync();
    }

    /// <summary>Ajoute un joueur à un salon (idempotent : ne duplique pas s'il en fait déjà partie).</summary>
    public async Task AddMemberAsync(int chatGroupId, int playerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var alreadyMember = await db.ChatGroupMembers
            .AnyAsync(m => m.ChatGroupId == chatGroupId && m.PlayerId == playerId);
        if (alreadyMember) return;

        db.ChatGroupMembers.Add(new ChatGroupMember { ChatGroupId = chatGroupId, PlayerId = playerId });
        await db.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int chatGroupId, int playerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var membership = await db.ChatGroupMembers
            .FirstOrDefaultAsync(m => m.ChatGroupId == chatGroupId && m.PlayerId == playerId);
        if (membership is null) return;

        db.ChatGroupMembers.Remove(membership);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Bascule un joueur éliminé vers le salon des morts : les règles prévoient
    /// un salon spécial où les morts échangent leurs informations et peuvent se
    /// mettre d'accord pour ajouter un vote bonus à un joueur restant.
    /// </summary>
    public async Task MoveToDeadGroupAsync(int playerId)
    {
        var deadGroup = await GetOrCreateSystemGroupAsync("Morts", "💀");
        await AddMemberAsync(deadGroup.Id, playerId);
    }

    // ---------- Messages ----------

    /// <summary>
    /// Enregistre un message dans un salon. senderPlayerId à null = message
    /// système (annonce du MJ : morts, résultats de vote, etc.).
    /// </summary>
    public async Task<ChatMessage> PostMessageAsync(int chatGroupId, int? senderPlayerId, string content, bool isSystemMessage = false)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var message = new ChatMessage
        {
            ChatGroupId = chatGroupId,
            SenderPlayerId = senderPlayerId,
            Content = content.Trim(),
            IsSystemMessage = isSystemMessage || senderPlayerId is null
        };
        db.ChatMessages.Add(message);
        await db.SaveChangesAsync();

        if (senderPlayerId is not null)
            message.SenderPlayer = await db.Players.FindAsync(senderPlayerId.Value);

        return message;
    }

    /// <summary>Raccourci pour poster une annonce système (ex: "Le village se réveille...").</summary>
    public Task<ChatMessage> PostSystemMessageAsync(int chatGroupId, string content) =>
        PostMessageAsync(chatGroupId, senderPlayerId: null, content, isSystemMessage: true);

    public async Task<List<ChatMessage>> GetMessagesAsync(int chatGroupId, int take = 200)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ChatMessages
            .Where(m => m.ChatGroupId == chatGroupId)
            .Include(m => m.SenderPlayer)
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    /// <summary>Vérifie si un salon est le salon "Village" (utile pour l'UI : pas besoin d'un bouton "rejoindre").</summary>
    public bool IsVillageGroup(ChatGroup group) => group.IsSystemGroup && group.Name == "Village";
}