using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddFieldTypeToValidationRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldType",
                table: "ValidationRules",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ValidationRules");
        }
    }
}
