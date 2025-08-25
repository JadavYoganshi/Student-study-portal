using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSP.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectToHomework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Students_StudentId",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Students_StudentId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Todos",
                newName: "S_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_StudentId",
                table: "Todos",
                newName: "IX_Todos_S_Id");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Homeworks",
                newName: "S_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Homeworks_StudentId",
                table: "Homeworks",
                newName: "IX_Homeworks_S_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Students_S_Id",
                table: "Homeworks",
                column: "S_Id",
                principalTable: "Students",
                principalColumn: "S_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Students_S_Id",
                table: "Todos",
                column: "S_Id",
                principalTable: "Students",
                principalColumn: "S_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Students_S_Id",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Students_S_Id",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "S_Id",
                table: "Todos",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_S_Id",
                table: "Todos",
                newName: "IX_Todos_StudentId");

            migrationBuilder.RenameColumn(
                name: "S_Id",
                table: "Homeworks",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Homeworks_S_Id",
                table: "Homeworks",
                newName: "IX_Homeworks_StudentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Todos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Students_StudentId",
                table: "Homeworks",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "S_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Students_StudentId",
                table: "Todos",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "S_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
