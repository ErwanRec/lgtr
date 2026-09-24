using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerNote> PlayerNotes => Set<PlayerNote>();
    public DbSet<RoleDefinition> RoleDefinitions => Set<RoleDefinition>();
    public DbSet<ChatGroup> ChatGroups => Set<ChatGroup>();
    public DbSet<ChatGroupMember> ChatGroupMembers => Set<ChatGroupMember>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<VoteSession> VoteSessions => Set<VoteSession>();
    public DbSet<VoteEntry> VoteEntries => Set<VoteEntry>();
    public DbSet<VoteBonus> VoteBonuses => Set<VoteBonus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>()
            .HasIndex(p => p.AccessCode)
            .IsUnique();

        modelBuilder.Entity<Player>()
            .HasOne(p => p.RoleDefinition)
            .WithMany()
            .HasForeignKey(p => p.RoleDefinitionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChatGroupMember>()
            .HasIndex(m => new { m.ChatGroupId, m.PlayerId })
            .IsUnique();

        modelBuilder.Entity<ChatGroupMember>()
            .HasOne(m => m.ChatGroup)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.ChatGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatGroupMember>()
            .HasOne(m => m.Player)
            .WithMany()
            .HasForeignKey(m => m.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.ChatGroup)
            .WithMany(g => g.Messages)
            .HasForeignKey(m => m.ChatGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.SenderPlayer)
            .WithMany()
            .HasForeignKey(m => m.SenderPlayerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<VoteEntry>()
            .HasIndex(v => new { v.VoteSessionId, v.VoterPlayerId })
            .IsUnique(); // "seul le dernier vote compte" -> on met à jour la même ligne

        modelBuilder.Entity<VoteEntry>()
            .HasOne(v => v.VoterPlayer)
            .WithMany()
            .HasForeignKey(v => v.VoterPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VoteEntry>()
            .HasOne(v => v.TargetPlayer)
            .WithMany()
            .HasForeignKey(v => v.TargetPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VoteBonus>()
            .HasOne(b => b.TargetPlayer)
            .WithMany()
            .HasForeignKey(b => b.TargetPlayerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<PlayerNote>()
            .HasIndex(n => new { n.AuthorPlayerId, n.TargetPlayerId })
            .IsUnique();

        modelBuilder.Entity<PlayerNote>()
            .HasOne(n => n.Author)
            .WithMany()
            .HasForeignKey(n => n.AuthorPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerNote>()
            .HasOne(n => n.Target)
            .WithMany()
            .HasForeignKey(n => n.TargetPlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}