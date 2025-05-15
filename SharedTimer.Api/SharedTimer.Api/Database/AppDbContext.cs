using Microsoft.EntityFrameworkCore;
using shared_timer_api.Entities;

namespace shared_timer_api.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<UserToGroup> UserToGroups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.DisplayName)
                .HasMaxLength(32);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => new { g.Name, g.OwnerId });

            entity.Property(g => g.Name)
                .HasMaxLength(32);

            entity.HasOne(g => g.Owner)
                .WithMany(u => u.OwnedGroups)
                .HasForeignKey(g => g.OwnerId);
        });

        modelBuilder.Entity<UserToGroup>(entity =>
        {
            entity.HasKey(ug => new { ug.UserId, ug.GroupName });

            entity.Property(ug => ug.GroupName)
                .HasMaxLength(32);

            entity.HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            entity.HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => new { ug.GroupName, ug.GroupOwnerId });
        });
    }
}