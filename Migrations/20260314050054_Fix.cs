using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ligas_Users_UserId",
                table: "Ligas");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Ligas",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ligas_UserId",
                table: "Ligas",
                newName: "IX_Ligas_CreatedByUserId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 5, 0, 53, 549, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 5, 0, 53, 549, DateTimeKind.Utc).AddTicks(5601));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 3, 14, 5, 0, 53, 666, DateTimeKind.Utc).AddTicks(2096), "$2a$11$1w1t3L0vrQVDH62LJ74Jv.7qhYnimlWEbhQdWsm0dxQKEUOPtNExC" });

            migrationBuilder.AddForeignKey(
                name: "FK_Ligas_Users_CreatedByUserId",
                table: "Ligas",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ligas_Users_CreatedByUserId",
                table: "Ligas");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Ligas",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ligas_CreatedByUserId",
                table: "Ligas",
                newName: "IX_Ligas_UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Ligas_Users_UserId",
                table: "Ligas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
