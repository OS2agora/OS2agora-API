using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BallerupKommune.DAOs.Persistence.Migrations
{
    public partial class AddCommentDeclineInfoEntityToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentDeclineInfoId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommentDeclineInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DeclineReason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeclinerInitials = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                    table.PrimaryKey("PK_CommentDeclineInfos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentDeclineInfoId",
                table: "Comments",
                column: "CommentDeclineInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentDeclineInfos_CommentDeclineInfoId",
                table: "Comments",
                column: "CommentDeclineInfoId",
                principalTable: "CommentDeclineInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            createCommentDeclineInfo(migrationBuilder);

            migrationBuilder.DropColumn(
                name: "CommentDeclineReason",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentDeclineInfos_CommentDeclineInfoId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CommentDeclineInfos");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentDeclineInfoId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentDeclineInfoId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "CommentDeclineReason",
                table: "Comments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        private void createCommentDeclineInfo(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE CommentDeclineInfos ADD COLUMN TempCommentId INT;

                INSERT INTO CommentDeclineInfos (DeclineReason, DeclinerInitials, Created, TempCommentId)
                SELECT DISTINCT 
                 c.CommentDeclineReason, 
                 u.EmployeeDisplayName, 
                 NOW(),
                 c.Id AS TempCommentId
                FROM Comments c
                JOIN UserHearingRoles uhr ON c.HearingId = uhr.HearingId
                JOIN UsersDb u ON uhr.UserId = u.Id
                JOIN HearingRoles hr ON uhr.HearingRoleId = hr.Id
                WHERE hr.Role = 1 AND c.CommentDeclineReason IS NOT NULL;

                UPDATE Comments c
                JOIN CommentDeclineInfos cdi ON c.Id = cdi.TempCommentId
                SET c.CommentDeclineInfoId = cdi.Id;

                ALTER TABLE CommentDeclineInfos DROP COLUMN TempCommentId;
                ");
        }
    }
}
