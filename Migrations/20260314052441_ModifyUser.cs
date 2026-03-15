using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "CreatedAt", "Password", "Username" },
                values: new object[] { new DateTime(2026, 3, 14, 5, 0, 53, 666, DateTimeKind.Utc).AddTicks(2096), "$2a$11$1w1t3L0vrQVDH62LJ74Jv.7qhYnimlWEbhQdWsm0dxQKEUOPtNExC", "admin" });
        }
    }
}
