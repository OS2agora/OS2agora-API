using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class CascadeDeleteNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications",
                column: "NotificationQueueId",
                principalTable: "NotificationQueues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications",
                column: "NotificationQueueId",
                principalTable: "NotificationQueues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
