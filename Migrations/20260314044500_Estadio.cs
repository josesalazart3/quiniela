using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class Estadio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estadio",
                table: "Partidos");

            migrationBuilder.AddColumn<int>(
                name: "EstadioId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Estadio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Ciudad = table.Column<string>(type: "text", nullable: false),
                    Pais = table.Column<string>(type: "text", nullable: false),
                    Capacidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadio", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 44, 59, 668, DateTimeKind.Utc).AddTicks(4758));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 44, 59, 668, DateTimeKind.Utc).AddTicks(4819));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 4, 44, 59, 785, DateTimeKind.Utc).AddTicks(5855), "$2a$11$R8Vq1Ea8nZW2FkGMolYEK.qn7NyajJFKjNN3QwICCg1JTQsB03Htu" });

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EstadioId",
                table: "Partidos",
                column: "EstadioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Estadio_EstadioId",
                table: "Partidos",
                column: "EstadioId",
                principalTable: "Estadio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Estadio_EstadioId",
                table: "Partidos");

            migrationBuilder.DropTable(
                name: "Estadio");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_EstadioId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "EstadioId",
                table: "Partidos");

            migrationBuilder.AddColumn<string>(
                name: "Estadio",
                table: "Partidos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 41, 10, 91, DateTimeKind.Utc).AddTicks(986));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 41, 10, 91, DateTimeKind.Utc).AddTicks(1048));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 4, 41, 10, 207, DateTimeKind.Utc).AddTicks(7007), "$2a$11$8IYWZbYBzKQKCiaXnVNm0u2GZWgl0HTFEuKCUPryWmTnzX/gzRlQy" });
        }
    }
}
