using Bwadl.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bwadl.Accounting.Infrastructure.Data.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(ak => ak.Id);
        
        builder.Property(ak => ak.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ak => ak.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ak => ak.KeyHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ak => ak.Description)
            .HasMaxLength(1000);

        builder.Property(ak => ak.UserId);

        builder.Property(ak => ak.ExpiresAt);

        builder.Property(ak => ak.LastUsedAt);

        builder.Property(ak => ak.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ak => ak.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ak => ak.RevokedAt);

        builder.Property(ak => ak.RevokedBy)
            .HasMaxLength(100);

        builder.Property(ak => ak.RevokedReason)
            .HasMaxLength(500);

        builder.Property(ak => ak.RateLimitPerMinute);

        builder.Property(ak => ak.RateLimitPerHour);

        builder.Property(ak => ak.RateLimitPerDay);

        builder.Property(ak => ak.AllowedIpAddresses)
            .HasColumnType("jsonb");

        builder.Property(ak => ak.Permissions)
            .HasColumnType("jsonb");

        // Versioned entity properties
        builder.Property(ak => ak.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(ak => ak.CreatedAt)
            .IsRequired();

        builder.Property(ak => ak.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ak => ak.UpdatedAt)
            .IsRequired();

        builder.Property(ak => ak.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(ak => ak.KeyHash)
            .IsUnique()
            .HasDatabaseName("IX_ApiKeys_KeyHash");

        builder.HasIndex(ak => ak.UserId)
            .HasDatabaseName("IX_ApiKeys_UserId");

        builder.HasIndex(ak => ak.IsActive)
            .HasDatabaseName("IX_ApiKeys_IsActive");

        builder.HasIndex(ak => ak.IsRevoked)
            .HasDatabaseName("IX_ApiKeys_IsRevoked");

        builder.HasIndex(ak => ak.ExpiresAt)
            .HasDatabaseName("IX_ApiKeys_ExpiresAt");

        builder.HasIndex(ak => ak.LastUsedAt)
            .HasDatabaseName("IX_ApiKeys_LastUsedAt");

        // Relationships
        builder.HasOne(ak => ak.User)
            .WithMany(u => u.ApiKeys)
            .HasForeignKey(ak => ak.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
