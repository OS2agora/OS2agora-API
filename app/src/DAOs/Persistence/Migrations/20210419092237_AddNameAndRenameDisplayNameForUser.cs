using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddNameAndRenameDisplayNameForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "UsersDb");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDisplayName",
                table: "UsersDb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UsersDb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeDisplayName",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UsersDb");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "UsersDb",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
