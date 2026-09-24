using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Data;

public static class DbInitializer
{
    /// <summary>
    /// Crée la base si besoin, insère le catalogue de rôles et un compte MJ
    /// par défaut (code d'accès "000") au tout premier lancement.
    /// </summary>
    public static void Initialize(AppDbContext db)
    {
        db.Database.EnsureCreated();

        var existingNames = db.RoleDefinitions
            .Select(r => r.Name)
            .ToHashSet();

        var missingRoles = RoleCatalogSeed.GetRoles()
            .Where(r => !existingNames.Contains(r.Name))
            .ToList();

        if (missingRoles.Count > 0)
        {
            db.RoleDefinitions.AddRange(missingRoles);
            db.SaveChanges();
        }

        if (!db.Players.Any(p => p.IsGameMaster))
        {
            db.Players.Add(new Player
            {
                FirstName = "Maître",
                LastName = "du Jeu",
                AccessCode = "000",
                IsGameMaster = true,
                IsAlive = true,
                Camp = Camp.Villageois
            });
            db.SaveChanges();
        }

        if (!db.ChatGroups.Any(g => g.Name == "Village"))
        {
            db.ChatGroups.Add(new ChatGroup
            {
                Name = "Village",
                Emoji = "🏘️",
                IsSystemGroup = true
            });
            db.SaveChanges();
        }
    }
}