using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class AddPenales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GolesLocalPenales",
                table: "Partidos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GolesVisitantePenales",
                table: "Partidos",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GolesLocalPenales",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "GolesVisitantePenales",
                table: "Partidos");
        }
    }
}
