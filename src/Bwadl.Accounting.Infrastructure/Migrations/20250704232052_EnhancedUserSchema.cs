using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bwadl.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mobile_country_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValue: "+966"),
                    identity_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    identity_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    identity_expiry_date = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    identity_date_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    session_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    device_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    name_ar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false, defaultValue: "en"),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_mobile_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_user_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    email_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    mobile_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_chosen_participant_id = table.Column<int>(type: "integer", nullable: true),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_created_at",
                table: "users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_by_user_id",
                table: "users",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_identity_id",
                table: "users",
                column: "identity_id",
                unique: true,
                filter: "identity_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_email_verified",
                table: "users",
                column: "is_email_verified");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_mobile_verified",
                table: "users",
                column: "is_mobile_verified");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_user_verified",
                table: "users",
                column: "is_user_verified");

            migrationBuilder.CreateIndex(
                name: "ix_users_mobile",
                table: "users",
                columns: new[] { "mobile", "mobile_country_code" },
                unique: true,
                filter: "mobile IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_session_id",
                table: "users",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_updated_at",
                table: "users",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_users_updated_by_user_id",
                table: "users",
                column: "updated_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
