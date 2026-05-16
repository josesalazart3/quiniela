using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class AddPremioDistribuidoYTorneoFinalizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Torneos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PremiosDistribuidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TorneoId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LigaId = table.Column<int>(type: "integer", nullable: true),
                    Concepto = table.Column<string>(type: "text", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Posicion = table.Column<int>(type: "integer", nullable: false),
                    FechaDistribucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiosDistribuidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiosDistribuidos_Ligas_LigaId",
                        column: x => x.LigaId,
                        principalTable: "Ligas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PremiosDistribuidos_Torneos_TorneoId",
                        column: x => x.TorneoId,
                        principalTable: "Torneos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PremiosDistribuidos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremiosDistribuidos_LigaId",
                table: "PremiosDistribuidos",
                column: "LigaId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiosDistribuidos_TorneoId",
                table: "PremiosDistribuidos",
                column: "TorneoId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiosDistribuidos_UserId",
                table: "PremiosDistribuidos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremiosDistribuidos");

            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Torneos");
        }
    }
}
