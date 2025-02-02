using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PiketWebApi.Migrations
{
    /// <inheritdoc />
    public partial class attendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PicketId",
                table: "StudentAttendaces",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendaces_PicketId",
                table: "StudentAttendaces",
                column: "PicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendaces_Picket_PicketId",
                table: "StudentAttendaces",
                column: "PicketId",
                principalTable: "Picket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendaces_Picket_PicketId",
                table: "StudentAttendaces");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendaces_PicketId",
                table: "StudentAttendaces");

            migrationBuilder.DropColumn(
                name: "PicketId",
                table: "StudentAttendaces");
        }
    }
}
