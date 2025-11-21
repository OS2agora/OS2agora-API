using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddedGlobalContentTypeMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GlobalContents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "GlobalContents");

            migrationBuilder.AddColumn<int>(
                name: "GlobalContentTypeId",
                table: "GlobalContents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GlobalContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalContentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalContents_GlobalContentTypeId",
                table: "GlobalContents",
                column: "GlobalContentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalContents_GlobalContentTypes_GlobalContentTypeId",
                table: "GlobalContents",
                column: "GlobalContentTypeId",
                principalTable: "GlobalContentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalContents_GlobalContentTypes_GlobalContentTypeId",
                table: "GlobalContents");

            migrationBuilder.DropTable(
                name: "GlobalContentTypes");

            migrationBuilder.DropIndex(
                name: "IX_GlobalContents_GlobalContentTypeId",
                table: "GlobalContents");

            migrationBuilder.DropColumn(
                name: "GlobalContentTypeId",
                table: "GlobalContents");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GlobalContents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "GlobalContents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
