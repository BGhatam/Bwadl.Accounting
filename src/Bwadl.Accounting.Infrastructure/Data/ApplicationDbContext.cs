using Bwadl.Accounting.Domain.Common;
using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null) 
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations in the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateVersionedEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateVersionedEntities();
        return base.SaveChanges();
    }

    private void UpdateVersionedEntities()
    {
        var currentUser = _currentUserService?.UserName ?? "System";
        
        var versionedEntries = ChangeTracker.Entries<IVersionedEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in versionedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Version = 1;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUser;
                    break;

                case EntityState.Modified:
                    entry.Entity.Version++;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUser;
                    // Prevent modification of CreatedAt and CreatedBy
                    entry.Property(nameof(IVersionedEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IVersionedEntity.CreatedBy)).IsModified = false;
                    break;
            }
        }
    }
}
