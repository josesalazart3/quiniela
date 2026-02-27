using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class InsertDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 27, 3, 58, 55, 693, DateTimeKind.Utc).AddTicks(1500), "SystemAdmin", null },
                    { 2, new DateTime(2026, 2, 27, 3, 58, 55, 693, DateTimeKind.Utc).AddTicks(1623), "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "Password", "RoleId", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2026, 2, 27, 3, 58, 55, 809, DateTimeKind.Utc).AddTicks(8076), "admin@quiniela.com", "System", "Admin", "$2a$11$Dho8Q8l6xXuvV.rZvLwACueoTEWodOBmKGWU.IHabuH0TBveA46li", 1, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
