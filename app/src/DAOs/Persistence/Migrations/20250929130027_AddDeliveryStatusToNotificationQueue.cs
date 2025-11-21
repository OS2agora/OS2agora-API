using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddDeliveryStatusToNotificationQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuccessfullSendDate",
                table: "NotificationQueues",
                newName: "SuccessfulSentDate");

            migrationBuilder.RenameColumn(
                name: "IsSend",
                table: "NotificationQueues",
                newName: "IsSent");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatus",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "NotificationQueues",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SentAs",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuccessfulDeliveryDate",
                table: "NotificationQueues",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_MessageId",
                table: "NotificationQueues",
                column: "MessageId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_MessageId",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "SentAs",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "SuccessfulDeliveryDate",
                table: "NotificationQueues");

            migrationBuilder.RenameColumn(
                name: "SuccessfulSentDate",
                table: "NotificationQueues",
                newName: "SuccessfullSendDate");

            migrationBuilder.RenameColumn(
                name: "IsSent",
                table: "NotificationQueues",
                newName: "IsSend");
        }
    }
}
