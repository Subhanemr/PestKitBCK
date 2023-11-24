using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesKit.Migrations
{
    public partial class Mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaceLink",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstLink",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedLink",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwitLink",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceLink",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "InstLink",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LinkedLink",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TwitLink",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Departments");
        }
    }
}
