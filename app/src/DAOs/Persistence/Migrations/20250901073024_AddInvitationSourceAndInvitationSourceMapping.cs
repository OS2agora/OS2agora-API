using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddInvitationSourceAndInvitationSourceMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvitationSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvitationSourceType = table.Column<int>(type: "int", nullable: false),
                    CanDeleteIndividuals = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CprColumnHeader = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailColumnHeader = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CvrColumnHeader = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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
                    table.PrimaryKey("PK_InvitationSources", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvitationSourceMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvitationSourceId = table.Column<int>(type: "int", nullable: false),
                    CompanyHearingRoleId = table.Column<int>(type: "int", nullable: true),
                    UserHearingRoleId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationSourceMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationSourceMappings_CompanyHearingRoles_CompanyHearingR~",
                        column: x => x.CompanyHearingRoleId,
                        principalTable: "CompanyHearingRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitationSourceMappings_InvitationSources_InvitationSourceId",
                        column: x => x.InvitationSourceId,
                        principalTable: "InvitationSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitationSourceMappings_UserHearingRoles_UserHearingRoleId",
                        column: x => x.UserHearingRoleId,
                        principalTable: "UserHearingRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationSourceMappings_CompanyHearingRoleId",
                table: "InvitationSourceMappings",
                column: "CompanyHearingRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationSourceMappings_InvitationSourceId",
                table: "InvitationSourceMappings",
                column: "InvitationSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationSourceMappings_UserHearingRoleId",
                table: "InvitationSourceMappings",
                column: "UserHearingRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitationSourceMappings");

            migrationBuilder.DropTable(
                name: "InvitationSources");
        }
    }
}
