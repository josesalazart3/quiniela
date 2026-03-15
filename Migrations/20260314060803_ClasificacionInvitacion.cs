using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class ClasificacionInvitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Estadio_EstadioId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Fases_FaseId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Torneos_TorneoId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prediccion_Ligas_LigaId",
                table: "Prediccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Prediccion_Partidos_PartidoId",
                table: "Prediccion");

            migrationBuilder.DropForeignKey(
                name: "FK_Prediccion_Users_UserId",
                table: "Prediccion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prediccion",
                table: "Prediccion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Estadio",
                table: "Estadio");

            migrationBuilder.RenameTable(
                name: "Prediccion",
                newName: "Predicciones");

            migrationBuilder.RenameTable(
                name: "Estadio",
                newName: "Estadios");

            migrationBuilder.RenameIndex(
                name: "IX_Prediccion_UserId_LigaId_PartidoId",
                table: "Predicciones",
                newName: "IX_Predicciones_UserId_LigaId_PartidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Prediccion_PartidoId",
                table: "Predicciones",
                newName: "IX_Predicciones_PartidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Prediccion_LigaId",
                table: "Predicciones",
                newName: "IX_Predicciones_LigaId");

            migrationBuilder.AlterColumn<int>(
                name: "EquipoVisitanteId",
                table: "Partidos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "EquipoLocalId",
                table: "Partidos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "DescripcionLocal",
                table: "Partidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescripcionVisitante",
                table: "Partidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "LigaMiembros",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Predicciones",
                table: "Predicciones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estadios",
                table: "Estadios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ClasificacionGrupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrupoId = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    PartidosJugados = table.Column<int>(type: "integer", nullable: false),
                    Ganados = table.Column<int>(type: "integer", nullable: false),
                    Empatados = table.Column<int>(type: "integer", nullable: false),
                    Perdidos = table.Column<int>(type: "integer", nullable: false),
                    GolesAFavor = table.Column<int>(type: "integer", nullable: false),
                    GolesEnContra = table.Column<int>(type: "integer", nullable: false),
                    DiferenciaGoles = table.Column<int>(type: "integer", nullable: false),
                    Puntos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasificacionGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClasificacionGrupos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClasificacionGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvitacionesLiga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LigaId = table.Column<int>(type: "integer", nullable: false),
                    EmailInvitado = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitacionesLiga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitacionesLiga_Ligas_LigaId",
                        column: x => x.LigaId,
                        principalTable: "Ligas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitacionesLiga_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "quiniela" });

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionGrupos_EquipoId",
                table: "ClasificacionGrupos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionGrupos_GrupoId_EquipoId",
                table: "ClasificacionGrupos",
                columns: new[] { "GrupoId", "EquipoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesLiga_LigaId",
                table: "InvitacionesLiga",
                column: "LigaId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesLiga_Token",
                table: "InvitacionesLiga",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesLiga_UserId",
                table: "InvitacionesLiga",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Estadios_EstadioId",
                table: "Partidos",
                column: "EstadioId",
                principalTable: "Estadios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Fases_FaseId",
                table: "Partidos",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Torneos_TorneoId",
                table: "Partidos",
                column: "TorneoId",
                principalTable: "Torneos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Predicciones_Ligas_LigaId",
                table: "Predicciones",
                column: "LigaId",
                principalTable: "Ligas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Predicciones_Partidos_PartidoId",
                table: "Predicciones",
                column: "PartidoId",
                principalTable: "Partidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Predicciones_Users_UserId",
                table: "Predicciones",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Estadios_EstadioId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Fases_FaseId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Torneos_TorneoId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Predicciones_Ligas_LigaId",
                table: "Predicciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Predicciones_Partidos_PartidoId",
                table: "Predicciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Predicciones_Users_UserId",
                table: "Predicciones");

            migrationBuilder.DropTable(
                name: "ClasificacionGrupos");

            migrationBuilder.DropTable(
                name: "InvitacionesLiga");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Predicciones",
                table: "Predicciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Estadios",
                table: "Estadios");

            migrationBuilder.DropColumn(
                name: "DescripcionLocal",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "DescripcionVisitante",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "LigaMiembros");

            migrationBuilder.RenameTable(
                name: "Predicciones",
                newName: "Prediccion");

            migrationBuilder.RenameTable(
                name: "Estadios",
                newName: "Estadio");

            migrationBuilder.RenameIndex(
                name: "IX_Predicciones_UserId_LigaId_PartidoId",
                table: "Prediccion",
                newName: "IX_Prediccion_UserId_LigaId_PartidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Predicciones_PartidoId",
                table: "Prediccion",
                newName: "IX_Prediccion_PartidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Predicciones_LigaId",
                table: "Prediccion",
                newName: "IX_Prediccion_LigaId");

            migrationBuilder.AlterColumn<int>(
                name: "EquipoVisitanteId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EquipoLocalId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prediccion",
                table: "Prediccion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estadio",
                table: "Estadio",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 5, 24, 40, 912, DateTimeKind.Utc).AddTicks(4924));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 5, 24, 40, 912, DateTimeKind.Utc).AddTicks(4985));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 5, 24, 41, 30, DateTimeKind.Utc).AddTicks(2708), "$2a$11$HxOK/pccSttau.awnLGHde42fCb7Re6AATQjWl8AaC8PPpS3lSOD6" });

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Estadio_EstadioId",
                table: "Partidos",
                column: "EstadioId",
                principalTable: "Estadio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Fases_FaseId",
                table: "Partidos",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Torneos_TorneoId",
                table: "Partidos",
                column: "TorneoId",
                principalTable: "Torneos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prediccion_Ligas_LigaId",
                table: "Prediccion",
                column: "LigaId",
                principalTable: "Ligas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prediccion_Partidos_PartidoId",
                table: "Prediccion",
                column: "PartidoId",
                principalTable: "Partidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prediccion_Users_UserId",
                table: "Prediccion",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
