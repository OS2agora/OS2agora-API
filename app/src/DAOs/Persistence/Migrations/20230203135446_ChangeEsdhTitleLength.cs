using Microsoft.EntityFrameworkCore.Migrations;

namespace BallerupKommune.DAOs.Persistence.Migrations
{
    public partial class ChangeEsdhTitleLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EsdhTitle",
                table: "Hearings",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100) CHARACTER SET utf8mb4",
                oldMaxLength: 100,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EsdhTitle",
                table: "Hearings",
                type: "varchar(100) CHARACTER SET utf8mb4",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 110,
                oldNullable: true);
        }
    }
}
