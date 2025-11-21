using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddCityArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityAreaId",
                table: "Hearings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CityAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityAreas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Hearings_CityAreaId",
                table: "Hearings",
                column: "CityAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearings_CityAreas_CityAreaId",
                table: "Hearings",
                column: "CityAreaId",
                principalTable: "CityAreas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearings_CityAreas_CityAreaId",
                table: "Hearings");

            migrationBuilder.DropTable(
                name: "CityAreas");

            migrationBuilder.DropIndex(
                name: "IX_Hearings_CityAreaId",
                table: "Hearings");

            migrationBuilder.DropColumn(
                name: "CityAreaId",
                table: "Hearings");
        }
    }
}
