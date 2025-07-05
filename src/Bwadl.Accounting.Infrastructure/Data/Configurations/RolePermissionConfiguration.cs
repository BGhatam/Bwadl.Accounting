using Bwadl.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bwadl.Accounting.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => rp.Id);
        
        builder.Property(rp => rp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rp => rp.RoleId)
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .IsRequired();

        builder.Property(rp => rp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Versioned entity properties
        builder.Property(rp => rp.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(rp => rp.CreatedAt)
            .IsRequired();

        builder.Property(rp => rp.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rp => rp.UpdatedAt)
            .IsRequired();

        builder.Property(rp => rp.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_Role_Permission");

        builder.HasIndex(rp => rp.RoleId)
            .HasDatabaseName("IX_RolePermissions_RoleId");

        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("IX_RolePermissions_PermissionId");

        builder.HasIndex(rp => rp.IsActive)
            .HasDatabaseName("IX_RolePermissions_IsActive");

        // Relationships
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
