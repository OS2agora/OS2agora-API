using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddedMoreValidationRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxFileCount",
                table: "ValidationRules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxFileSize",
                table: "ValidationRules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxFileCount",
                table: "ValidationRules");

            migrationBuilder.DropColumn(
                name: "MaxFileSize",
                table: "ValidationRules");
        }
    }
}
