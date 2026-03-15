using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class LigaAndTorneo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Torneos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ligas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    EsDeApuestas = table.Column<bool>(type: "boolean", nullable: false),
                    PrecioPorUnirse = table.Column<decimal>(type: "numeric", nullable: true),
                    TorneoId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ligas_Torneos_TorneoId",
                        column: x => x.TorneoId,
                        principalTable: "Torneos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ligas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LigaMiembros",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LigaId = table.Column<int>(type: "integer", nullable: false),
                    EsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    Puntos = table.Column<int>(type: "integer", nullable: false),
                    FechaUnion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LigaMiembros", x => new { x.UserId, x.LigaId });
                    table.ForeignKey(
                        name: "FK_LigaMiembros_Ligas_LigaId",
                        column: x => x.LigaId,
                        principalTable: "Ligas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LigaMiembros_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 3, 44, 58, 44, DateTimeKind.Utc).AddTicks(7134));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 3, 44, 58, 44, DateTimeKind.Utc).AddTicks(7211));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 3, 44, 58, 161, DateTimeKind.Utc).AddTicks(5862), "$2a$11$xC0oxge0L2Wwjzy3ZUI8TefV1SKu7nDyzvArYeam/ThyhlONtzD/2" });

            migrationBuilder.CreateIndex(
                name: "IX_LigaMiembros_LigaId",
                table: "LigaMiembros",
                column: "LigaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ligas_TorneoId",
                table: "Ligas",
                column: "TorneoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ligas_UserId",
                table: "Ligas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LigaMiembros");

            migrationBuilder.DropTable(
                name: "Ligas");

            migrationBuilder.DropTable(
                name: "Torneos");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 3, 58, 55, 693, DateTimeKind.Utc).AddTicks(1500));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 3, 58, 55, 693, DateTimeKind.Utc).AddTicks(1623));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 2, 27, 3, 58, 55, 809, DateTimeKind.Utc).AddTicks(8076), "$2a$11$Dho8Q8l6xXuvV.rZvLwACueoTEWodOBmKGWU.IHabuH0TBveA46li" });
        }
    }
}
