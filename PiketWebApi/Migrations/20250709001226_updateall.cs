using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiketWebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentPhoneNumber",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolYearId",
                table: "Picket",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Picket_SchoolYearId",
                table: "Picket",
                column: "SchoolYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Picket_SchoolYears_SchoolYearId",
                table: "Picket",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Picket_SchoolYears_SchoolYearId",
                table: "Picket");

            migrationBuilder.DropIndex(
                name: "IX_Picket_SchoolYearId",
                table: "Picket");

            migrationBuilder.DropColumn(
                name: "ParentPhoneNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SchoolYearId",
                table: "Picket");
        }
    }
}
