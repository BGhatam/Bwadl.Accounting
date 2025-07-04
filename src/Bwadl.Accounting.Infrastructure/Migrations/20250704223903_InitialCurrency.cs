using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bwadl.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    currency_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    decimal_places = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_currencies_created_at",
                table: "currencies",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_currency_code",
                table: "currencies",
                column: "currency_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_currencies_updated_at",
                table: "currencies",
                column: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies");
        }
    }
}
