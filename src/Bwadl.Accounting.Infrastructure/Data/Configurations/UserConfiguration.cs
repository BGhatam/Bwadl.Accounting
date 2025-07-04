using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bwadl.Accounting.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("users");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        // Email
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("email IS NOT NULL")
            .HasDatabaseName("ix_users_email");

        // Mobile (owned entity)
        builder.OwnsOne(u => u.Mobile, mobile =>
        {
            mobile.Property(m => m.Number)
                .HasColumnName("mobile")
                .HasMaxLength(20)
                .IsRequired(false);

            mobile.Property(m => m.CountryCode)
                .HasColumnName("mobile_country_code")
                .HasMaxLength(10)
                .HasDefaultValue("+966")
                .IsRequired(false);
                
            // Index within the owned entity
            mobile.HasIndex(m => new { m.Number, m.CountryCode })
                .IsUnique()
                .HasFilter("mobile IS NOT NULL")
                .HasDatabaseName("ix_users_mobile");
        });

        // Identity (owned entity)
        builder.OwnsOne(u => u.Identity, identity =>
        {
            identity.Property(i => i.Id)
                .HasColumnName("identity_id")
                .HasMaxLength(50)
                .IsRequired(false);

            identity.Property(i => i.Type)
                .HasColumnName("identity_type")
                .HasConversion<string>()
                .HasMaxLength(10);

            identity.Property(i => i.ExpiryDate)
                .HasColumnName("identity_expiry_date")
                .HasMaxLength(20)
                .IsRequired(false);

            identity.Property(i => i.DateType)
                .HasColumnName("identity_date_type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired(false);
                
            // Index within the owned entity
            identity.HasIndex(i => i.Id)
                .IsUnique()
                .HasFilter("identity_id IS NOT NULL")
                .HasDatabaseName("ix_users_identity_id");
        });

        // Session and device
        builder.Property(u => u.SessionId)
            .HasColumnName("session_id")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.DeviceToken)
            .HasColumnName("device_token")
            .HasMaxLength(500)
            .IsRequired(false);

        // Password
        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired(false);

        // Names
        builder.Property(u => u.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255)
            .IsRequired(false);

        // Language
        builder.Property(u => u.Language)
            .HasColumnName("language")
            .HasConversion(
                l => l.ToCode(),
                s => LanguageExtensions.FromCode(s))
            .HasMaxLength(5)
            .HasDefaultValue(Language.English);

        // Verification flags
        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasDefaultValue(false);

        builder.Property(u => u.IsMobileVerified)
            .HasColumnName("is_mobile_verified")
            .HasDefaultValue(false);

        builder.Property(u => u.IsUserVerified)
            .HasColumnName("is_user_verified")
            .HasDefaultValue(false);

        // Verification timestamps
        builder.Property(u => u.EmailVerifiedAt)
            .HasColumnName("email_verified_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(u => u.MobileVerifiedAt)
            .HasColumnName("mobile_verified_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(u => u.UserVerifiedAt)
            .HasColumnName("user_verified_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        // References
        builder.Property(u => u.LastChosenParticipantId)
            .HasColumnName("last_chosen_participant_id")
            .IsRequired(false);

        builder.Property(u => u.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired(false);

        builder.Property(u => u.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .IsRequired(false);

        // Additional data
        builder.Property(u => u.Details)
            .HasColumnName("details")
            .HasColumnType("jsonb")
            .IsRequired(false);

        // Version for optimistic concurrency
        builder.Property(u => u.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();

        // Audit fields
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(u => u.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100)
            .IsRequired();

        // Self-referencing relationships
        builder.HasOne(u => u.CreatedByUser)
            .WithMany()
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.UpdatedByUser)
            .WithMany()
            .HasForeignKey(u => u.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(u => u.SessionId)
            .HasDatabaseName("ix_users_session_id");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("ix_users_created_at");

        builder.HasIndex(u => u.UpdatedAt)
            .HasDatabaseName("ix_users_updated_at");

        builder.HasIndex(u => u.IsEmailVerified)
            .HasDatabaseName("ix_users_is_email_verified");

        builder.HasIndex(u => u.IsMobileVerified)
            .HasDatabaseName("ix_users_is_mobile_verified");

        builder.HasIndex(u => u.IsUserVerified)
            .HasDatabaseName("ix_users_is_user_verified");
    }
}
