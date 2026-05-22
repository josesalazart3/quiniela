using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToFaseAndGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Grupos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Grupos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Fases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Fases",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Fases");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Fases");
        }
    }
}
