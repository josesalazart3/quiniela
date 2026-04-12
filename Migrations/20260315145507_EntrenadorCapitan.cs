using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class EntrenadorCapitan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Capitan",
                table: "Equipos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Entrenador",
                table: "Equipos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capitan",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Entrenador",
                table: "Equipos");
        }
    }
}
