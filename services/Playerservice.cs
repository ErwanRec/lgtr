using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Data;
using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Services;

public class PlayerService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PlayerService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Player?> FindByAccessCodeAsync(string code)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Players.FirstOrDefaultAsync(p => p.AccessCode == code.Trim());
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Players
            .Include(p => p.RoleDefinition)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Player>> GetAllAsync(bool includeGameMasters = false)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.Players.Include(p => p.RoleDefinition).AsQueryable();
        if (!includeGameMasters) query = query.Where(p => !p.IsGameMaster);
        return await query.OrderBy(p => p.LastName).ToListAsync();
    }

    public async Task<Player> CreateAsync(string firstName, string lastName, string accessCode, bool isGameMaster = false)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var player = new Player
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            AccessCode = accessCode.Trim(),
            IsGameMaster = isGameMaster
        };
        db.Players.Add(player);
        await db.SaveChangesAsync();
        return player;
    }

    public async Task UpdateAsync(int playerId, Action<Player> mutate)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var player = await db.Players.FirstOrDefaultAsync(p => p.Id == playerId);
        if (player is null) return;
        mutate(player);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int playerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var player = await db.Players.FirstOrDefaultAsync(p => p.Id == playerId);
        if (player is null) return;
        db.Players.Remove(player);
        await db.SaveChangesAsync();
    }

    public async Task<List<RoleDefinition>> GetRolesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.RoleDefinitions.OrderBy(r => r.Camp).ThenBy(r => r.Name).ToListAsync();
    }
}