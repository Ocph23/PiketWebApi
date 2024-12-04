using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiketWebApi.Migrations
{
    /// <inheritdoc />
    public partial class senester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolYears_Year",
                table: "SchoolYears");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYears_Year_Semester",
                table: "SchoolYears",
                columns: new[] { "Year", "Semester" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolYears_Year_Semester",
                table: "SchoolYears");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYears_Year",
                table: "SchoolYears",
                column: "Year",
                unique: true);
        }
    }
}
