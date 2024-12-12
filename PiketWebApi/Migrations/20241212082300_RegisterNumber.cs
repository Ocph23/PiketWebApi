using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiketWebApi.Migrations
{
    /// <inheritdoc />
    public partial class RegisterNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Teachers",
                newName: "RegisterNumber");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Students",
                newName: "NISN");

            migrationBuilder.AddColumn<string>(
                name: "NIS",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Students_NIS",
                table: "Students",
                column: "NIS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_NIS",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "NIS",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "RegisterNumber",
                table: "Teachers",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "NISN",
                table: "Students",
                newName: "Number");
        }
    }
}
