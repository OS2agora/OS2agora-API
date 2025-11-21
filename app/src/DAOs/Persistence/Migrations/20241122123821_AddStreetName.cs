using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddStreetName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "UsersDb",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Companies",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Companies");
        }
    }
}
