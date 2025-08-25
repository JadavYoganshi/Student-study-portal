using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSP.Migrations
{
    /// <inheritdoc />
    public partial class AddSIdToTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeworkId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "TodoId",
                table: "Records");

            migrationBuilder.AddColumn<Guid>(
                name: "S_Id",
                table: "Records",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "S_Id",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "HomeworkId",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TodoId",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
