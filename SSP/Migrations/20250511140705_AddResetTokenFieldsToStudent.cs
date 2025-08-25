using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSP.Migrations
{
    /// <inheritdoc />
    public partial class AddResetTokenFieldsToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "S_Id",
                keyValue: new Guid("3d6fa47d-1f9d-4d77-8e2d-f20c2584edda"),
                columns: new[] { "ResetToken", "ResetTokenExpiry" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Students");
        }
    }
}
