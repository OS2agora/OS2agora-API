using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BallerupKommune.DAOs.Persistence.Migrations
{
    public partial class AddJournalizedStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JournalizedStatusId",
                table: "Hearings",
                nullable: true);

            migrationBuilder.Sql(@"
                Update Hearings SET JournalizedStatusId =
                    CASE 
                        WHEN IsJournalized = 0 THEN 1
                        ELSE 2
                    END;
            ");

            migrationBuilder.DropColumn(
                name: "IsJournalized",
                table: "Hearings");

            migrationBuilder.CreateTable(
                name: "JournalizedStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalizedStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "JournalizedStatuses",
                columns: new[] { "Id", "Created", "CreatedBy", "LastModified", "LastModifiedBy", "Status" },
                values: new object[] { 1, new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), null, null, null, 0 });

            migrationBuilder.InsertData(
                table: "JournalizedStatuses",
                columns: new[] { "Id", "Created", "CreatedBy", "LastModified", "LastModifiedBy", "Status" },
                values: new object[] { 2, new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), null, null, null, 1 });

            migrationBuilder.InsertData(
                table: "JournalizedStatuses",
                columns: new[] { "Id", "Created", "CreatedBy", "LastModified", "LastModifiedBy", "Status" },
                values: new object[] { 3, new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), null, null, null, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Hearings_JournalizedStatusId",
                table: "Hearings",
                column: "JournalizedStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearings_JournalizedStatuses_JournalizedStatusId",
                table: "Hearings",
                column: "JournalizedStatusId",
                principalTable: "JournalizedStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearings_JournalizedStatuses_JournalizedStatusId",
                table: "Hearings");

            migrationBuilder.DropTable(
                name: "JournalizedStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Hearings_JournalizedStatusId",
                table: "Hearings");

            migrationBuilder.AddColumn<bool>(
                name: "IsJournalized",
                table: "Hearings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                Update Hearings SET IsJournalized = 
                    CASE 
                        WHEN JournalizedStatusId = 2 THEN 1
                        ELSE 0
                    END;
            ");

            migrationBuilder.DropColumn(
                name: "JournalizedStatusId",
                table: "Hearings");
        }
    }
}
