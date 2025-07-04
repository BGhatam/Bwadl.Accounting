using Bwadl.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bwadl.Accounting.Infrastructure.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        // Table name
        builder.ToTable("currencies");

        // Primary key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        // Natural key (business key)
        builder.Property(c => c.CurrencyCode)
            .HasColumnName("currency_code")
            .HasMaxLength(3)
            .IsRequired();
        
        // Unique index on currency code
        builder.HasIndex(c => c.CurrencyCode)
            .IsUnique()
            .HasDatabaseName("ix_currencies_currency_code");

        // Properties
        builder.Property(c => c.CurrencyName)
            .HasColumnName("currency_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DecimalPlaces)
            .HasColumnName("decimal_places")
            .HasDefaultValue(2);

        // Version for optimistic concurrency
        builder.Property(c => c.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100)
            .IsRequired();

        // Index on audit fields for performance
        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("ix_currencies_created_at");

        builder.HasIndex(c => c.UpdatedAt)
            .HasDatabaseName("ix_currencies_updated_at");
    }
}
