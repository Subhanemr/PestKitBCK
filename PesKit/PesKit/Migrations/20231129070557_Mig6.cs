using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesKit.Migrations
{
    public partial class Mig6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlideImage_Slides_SlideId",
                table: "SlideImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SlideImage",
                table: "SlideImage");

            migrationBuilder.RenameTable(
                name: "SlideImage",
                newName: "SlideImages");

            migrationBuilder.RenameIndex(
                name: "IX_SlideImage_SlideId",
                table: "SlideImages",
                newName: "IX_SlideImages_SlideId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SlideImages",
                table: "SlideImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SlideImages_Slides_SlideId",
                table: "SlideImages",
                column: "SlideId",
                principalTable: "Slides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlideImages_Slides_SlideId",
                table: "SlideImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SlideImages",
                table: "SlideImages");

            migrationBuilder.RenameTable(
                name: "SlideImages",
                newName: "SlideImage");

            migrationBuilder.RenameIndex(
                name: "IX_SlideImages_SlideId",
                table: "SlideImage",
                newName: "IX_SlideImage_SlideId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SlideImage",
                table: "SlideImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SlideImage_Slides_SlideId",
                table: "SlideImage",
                column: "SlideId",
                principalTable: "Slides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
