using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTorneo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Anio",
                table: "Torneos",
                newName: "Año");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 3, 48, 4, 820, DateTimeKind.Utc).AddTicks(2643));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 3, 48, 4, 820, DateTimeKind.Utc).AddTicks(2724));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 3, 48, 4, 939, DateTimeKind.Utc).AddTicks(7427), "$2a$11$bl71EQlyWbl0zArg9wWpJuoAtZDoAgpP.8Wrb2Dc1OlmKpIMmpO3C" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Año",
                table: "Torneos",
                newName: "Anio");

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
        }
    }
}
