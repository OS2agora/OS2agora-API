using Microsoft.EntityFrameworkCore.Migrations;

namespace BallerupKommune.DAOs.Persistence.Migrations
{
    public partial class AddNotificationTypeEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "NotificationTypes");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NotificationTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "NotificationTypes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "NotificationTypes");

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "NotificationTypes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
