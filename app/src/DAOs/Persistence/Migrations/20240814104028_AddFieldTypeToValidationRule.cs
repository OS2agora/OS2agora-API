using Microsoft.EntityFrameworkCore.Migrations;

namespace BallerupKommune.DAOs.Persistence.Migrations
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

            // Update the FieldType on existing "Image" ValidationRule to correct FieldType
            migrationBuilder.Sql(@"UPDATE ValidationRules SET FieldType = 6 WHERE Id = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ValidationRules");
        }
    }
}

