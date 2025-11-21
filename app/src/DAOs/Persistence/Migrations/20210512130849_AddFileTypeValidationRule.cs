using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddFileTypeValidationRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedFileTypes",
                table: "ValidationRules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedFileTypes",
                table: "ValidationRules");
        }
    }
}
