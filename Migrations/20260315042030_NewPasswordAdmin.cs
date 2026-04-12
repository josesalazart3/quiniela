using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiniela.Migrations
{
    /// <inheritdoc />
    public partial class NewPasswordAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$D13huptFPV/i1px.II67.uvGztGXJfYqusE2hahkgCFJ0R3oPVVre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "quiniela");
        }
    }
}
