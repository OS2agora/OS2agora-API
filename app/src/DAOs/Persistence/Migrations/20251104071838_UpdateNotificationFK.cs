using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class UpdateNotificationFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_CompanyId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
