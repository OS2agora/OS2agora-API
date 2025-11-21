using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class UpdateDeletionBehaviourForEventsAndNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_UsersDb_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Comments_CommentEntityId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CommentEntityId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CommentEntityId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_UsersDb_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "UsersDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_UsersDb_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UsersDb_CompanyId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "CommentEntityId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentEntityId",
                table: "Notifications",
                column: "CommentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_UsersDb_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Comments_CommentEntityId",
                table: "Notifications",
                column: "CommentEntityId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UsersDb_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "UsersDb",
                principalColumn: "Id");
        }
    }
}
