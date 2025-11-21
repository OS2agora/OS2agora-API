using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddCompanyAndCompanyHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "UsersDb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpr",
                table: "UsersDb",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cvr",
                table: "UsersDb",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserCapacityId",
                table: "UsersDb",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 50, nullable: true),
                    Cvr = table.Column<string>(maxLength: 500, nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyHearingRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 50, nullable: true),
                    HearingRoleId = table.Column<int>(nullable: false),
                    HearingId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyHearingRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyHearingRoles_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyHearingRoles_Hearings_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyHearingRoles_HearingRoles_HearingRoleId",
                        column: x => x.HearingRoleId,
                        principalTable: "HearingRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersDb_CompanyId",
                table: "UsersDb",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersDb_UserCapacityId",
                table: "UsersDb",
                column: "UserCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHearingRoles_CompanyId",
                table: "CompanyHearingRoles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHearingRoles_HearingId",
                table: "CompanyHearingRoles",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHearingRoles_HearingRoleId",
                table: "CompanyHearingRoles",
                column: "HearingRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersDb_Companies_CompanyId",
                table: "UsersDb",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersDb_UserCapacities_UserCapacityId",
                table: "UsersDb",
                column: "UserCapacityId",
                principalTable: "UserCapacities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            mapEmployeesAndCitizens(migrationBuilder);
            mapCompanies(migrationBuilder);
            mapCompanyHearingRoles(migrationBuilder);
            deleteMappedCompanyUsers(migrationBuilder);

            migrationBuilder.DropTable(
                name: "UserCapacityMappings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersDb_Companies_CompanyId",
                table: "UsersDb");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersDb_UserCapacities_UserCapacityId",
                table: "UsersDb");

            migrationBuilder.DropTable(
                name: "CompanyHearingRoles");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_UsersDb_CompanyId",
                table: "UsersDb");

            migrationBuilder.DropIndex(
                name: "IX_UsersDb_UserCapacityId",
                table: "UsersDb");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "Cpr",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "Cvr",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "UserCapacityId",
                table: "UsersDb");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserCapacityMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: true),
                    UserCapacityId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCapacityMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCapacityMappings_UserCapacities_UserCapacityId",
                        column: x => x.UserCapacityId,
                        principalTable: "UserCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCapacityMappings_UsersDb_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCapacityMappings_UserCapacityId",
                table: "UserCapacityMappings",
                column: "UserCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCapacityMappings_UserId",
                table: "UserCapacityMappings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        private void mapEmployeesAndCitizens(MigrationBuilder migrationBuilder)
        {

            // Map employees - users with an email
            migrationBuilder.Sql(@"
                UPDATE UsersDb
                SET CPR = NULL, CVR = NULL, PersonalIdentifier = Email,
                    UserCapacityId = (SELECT Id FROM UserCapacities WHERE Capacity = 1)
                WHERE Email IS NOT NULL;
            ");

            // map citizens - users without an email and with a personal identifier consisting of 10 digits
            migrationBuilder.Sql(@"
                UPDATE UsersDb
                SET CPR = PersonalIdentifier, Email = NULL, CVR = NULL,
                    UserCapacityId = (SELECT Id FROM UserCapacities WHERE Capacity = 2)
                WHERE Email IS NULL AND LENGTH(PersonalIdentifier) = 10 AND PersonalIdentifier REGEXP '^[0-9]+$';
            ");

            // Update AspNetUsers UserName to match PersonalIdentifier for the updated Users
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers aUser
                JOIN UsersDb user ON aUser.Id = user.Identifier
                SET aUser.UserName = user.PersonalIdentifier, aUser.NormalizedUserName = user.PersonalIdentifier
                WHERE user.Email IS NOT NULL
                OR (user.Email IS NULL AND LENGTH(user.PersonalIdentifier) = 10 AND user.PersonalIdentifier REGEXP '^[0-9]+$');
            ");
        }

        private void mapCompanies(MigrationBuilder migrationBuilder)
        {
            // map companies - remaining users
            migrationBuilder.Sql(@"
                INSERT INTO Companies (Created, Cvr)
                SELECT NOW(), user.PersonalIdentifier
                FROM UsersDb user
                WHERE user.Email IS NULL AND (LENGTH(user.PersonalIdentifier) = 8 OR user.PersonalIdentifier NOT REGEXP '^[0-9]+$');
            ");
        }

        private void mapCompanyHearingRoles(MigrationBuilder migrationBuilder)
        {
            // map userHearingRoles to CompanyHearingRoles
            migrationBuilder.Sql(@"
                INSERT INTO CompanyHearingRoles (Created, HearingRoleId, HearingId, CompanyId)
                SELECT NOW(), uhr.HearingRoleId, uhr.HearingId, company.Id
                FROM UserHearingRoles uhr
                JOIN UsersDb user ON uhr.UserId = user.Id
                JOIN HearingRoles hr ON uhr.HearingRoleId = hr.Id
                JOIN Companies company ON company.Cvr = user.PersonalIdentifier
                WHERE user.Email IS NULL 
                AND (LENGTH(user.PersonalIdentifier) = 8 OR user.PersonalIdentifier NOT REGEXP '^[0-9]+$')
                AND hr.Role = 2
                AND NOT EXISTS (
                    SELECT 1 
                    FROM CompanyHearingRoles chr 
                    WHERE chr.HearingId = uhr.HearingId 
                    AND chr.CompanyId = company.Id 
                    AND chr.HearingRoleId = uhr.HearingRoleId
                );
            ");
        }

        private void deleteMappedCompanyUsers(MigrationBuilder migrationBuilder)
        {
            // Delete related RefreshTokens for the users mapped to companies
            migrationBuilder.Sql(@"
                DELETE FROM RefreshTokens
                WHERE ApplicationUserId IN (
                    SELECT user.Identifier 
                    FROM UsersDb user
                    WHERE user.Email IS NULL 
                    AND (LENGTH(user.PersonalIdentifier) = 8 OR user.PersonalIdentifier NOT REGEXP '^[0-9]+$')
                );
            ");

            // Delete Users and corresponding AspNetUsers for those mapped to companies
            migrationBuilder.Sql(@"
                DELETE FROM AspNetUsers
                WHERE Id IN (
                    SELECT user.Identifier 
                    FROM UsersDb user
                    WHERE user.Email IS NULL 
                    AND (LENGTH(user.PersonalIdentifier) = 8 OR user.PersonalIdentifier NOT REGEXP '^[0-9]+$')
                );
            ");

            // Delete the Users from UsersDb
            migrationBuilder.Sql(@"
                DELETE FROM UsersDb
                WHERE Email IS NULL 
                AND (LENGTH(PersonalIdentifier) = 8 OR PersonalIdentifier NOT REGEXP '^[0-9]+$');
            ");
        }
    }
}
