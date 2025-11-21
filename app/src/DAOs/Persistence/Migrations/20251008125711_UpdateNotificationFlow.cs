using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class UpdateNotificationFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Hearings_HearingId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_NotificationTemplate~",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTypes_NotificationTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationQueueId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateText",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "SubjectTemplate",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "NotificationQueueId",
                table: "Notifications");

            cleanUpData(migrationBuilder);

            migrationBuilder.RenameColumn(
                name: "IsSendToQueue",
                table: "Notifications",
                newName: "IsSentToQueue");

            migrationBuilder.AddColumn<int>(
                name: "SubjectTemplateId",
                table: "NotificationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BodyTemplateId",
                table: "NotificationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FooterTemplateId",
                table: "NotificationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeaderTemplateId",
                table: "NotificationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NotificationTemplates",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NotificationContentTypeId",
                table: "NotificationTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextContent",
                table: "NotificationTemplates",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HearingId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CommentEntityId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationId",
                table: "NotificationQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsSentInNotification = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    HearingId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Hearings_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_UsersDb_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersDb",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationContentTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMappings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventMappings_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TextContent = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotificationContentTypeId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationContents_NotificationContentTypes_NotificationCo~",
                        column: x => x.NotificationContentTypeId,
                        principalTable: "NotificationContentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationContentSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HeaderContentId = table.Column<int>(type: "int", nullable: true),
                    BodyContentId = table.Column<int>(type: "int", nullable: true),
                    FooterContentId = table.Column<int>(type: "int", nullable: true),
                    SubjectContentId = table.Column<int>(type: "int", nullable: true),
                    HearingId = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationContentSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_Hearings_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_NotificationContents_BodyC~",
                        column: x => x.BodyContentId,
                        principalTable: "NotificationContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_NotificationContents_Foote~",
                        column: x => x.FooterContentId,
                        principalTable: "NotificationContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_NotificationContents_Heade~",
                        column: x => x.HeaderContentId,
                        principalTable: "NotificationContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_NotificationContents_Subje~",
                        column: x => x.SubjectContentId,
                        principalTable: "NotificationContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationContentSpecifications_NotificationTypes_Notifica~",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypes_BodyTemplateId",
                table: "NotificationTypes",
                column: "BodyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypes_FooterTemplateId",
                table: "NotificationTypes",
                column: "FooterTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypes_HeaderTemplateId",
                table: "NotificationTypes",
                column: "HeaderTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypes_SubjectTemplateId",
                table: "NotificationTypes",
                column: "SubjectTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_NotificationContentTypeId",
                table: "NotificationTemplates",
                column: "NotificationContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentEntityId",
                table: "Notifications",
                column: "CommentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueues_NotificationId",
                table: "NotificationQueues",
                column: "NotificationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventMappings_EventId",
                table: "EventMappings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMappings_NotificationId",
                table: "EventMappings",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_HearingId",
                table: "Events",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_NotificationTypeId",
                table: "Events",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContents_NotificationContentTypeId",
                table: "NotificationContents",
                column: "NotificationContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_BodyContentId",
                table: "NotificationContentSpecifications",
                column: "BodyContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_FooterContentId",
                table: "NotificationContentSpecifications",
                column: "FooterContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_HeaderContentId",
                table: "NotificationContentSpecifications",
                column: "HeaderContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_HearingId",
                table: "NotificationContentSpecifications",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_NotificationTypeId",
                table: "NotificationContentSpecifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentSpecifications_SubjectContentId",
                table: "NotificationContentSpecifications",
                column: "SubjectContentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueues_Notifications_NotificationId",
                table: "NotificationQueues",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Comments_CommentEntityId",
                table: "Notifications",
                column: "CommentEntityId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Hearings_HearingId",
                table: "Notifications",
                column: "HearingId",
                principalTable: "Hearings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTemplates_NotificationContentTypes_NotificationC~",
                table: "NotificationTemplates",
                column: "NotificationContentTypeId",
                principalTable: "NotificationContentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_BodyTemplateId",
                table: "NotificationTypes",
                column: "BodyTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_FooterTemplateId",
                table: "NotificationTypes",
                column: "FooterTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_HeaderTemplateId",
                table: "NotificationTypes",
                column: "HeaderTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_SubjectTemplateId",
                table: "NotificationTypes",
                column: "SubjectTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueues_Notifications_NotificationId",
                table: "NotificationQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Comments_CommentEntityId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Hearings_HearingId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTemplates_NotificationContentTypes_NotificationC~",
                table: "NotificationTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_BodyTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_FooterTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_HeaderTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_SubjectTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropTable(
                name: "EventMappings");

            migrationBuilder.DropTable(
                name: "NotificationContentSpecifications");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "NotificationContents");

            migrationBuilder.DropTable(
                name: "NotificationContentTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTypes_BodyTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTypes_FooterTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTypes_HeaderTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTypes_SubjectTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_NotificationContentTypeId",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CommentEntityId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueues_NotificationId",
                table: "NotificationQueues");

            migrationBuilder.DropColumn(
                name: "BodyTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "FooterTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "HeaderTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "SubjectTemplateId",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "NotificationContentTypeId",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "TextContent",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "CommentEntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSentToQueue",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "NotificationQueues");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId",
                table: "NotificationTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "NotificationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NotificationTemplateText",
                table: "NotificationTemplates",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SubjectTemplate",
                table: "NotificationTemplates",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HearingId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationQueueId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "IsSentToQueue",
                table: "Notifications",
                newName: "IsSendToQueue");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypes_NotificationTemplateId",
                table: "NotificationTypes",
                column: "NotificationTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationQueueId",
                table: "Notifications",
                column: "NotificationQueueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Hearings_HearingId",
                table: "Notifications",
                column: "HearingId",
                principalTable: "Hearings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationQueues_NotificationQueueId",
                table: "Notifications",
                column: "NotificationQueueId",
                principalTable: "NotificationQueues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypes_NotificationTemplates_NotificationTemplate~",
                table: "NotificationTypes",
                column: "NotificationTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id");
        }

        private void cleanUpData(MigrationBuilder builder)
        {
            builder.Sql(@"
                DELETE FROM Notifications;
                DELETE FROM NotificationQueues;
                DELETE FROM NotificationTypes;
                DELETE FROM NotificationTemplates;
                
                ALTER TABLE Notifications AUTO_INCREMENT=1;
                ALTER TABLE NotificationQueues AUTO_INCREMENT=1;
                ALTER TABLE NotificationTypes AUTO_INCREMENT=1;
                ALTER TABLE NotificationTemplates AUTO_INCREMENT=1;
            ");
        }
    }
}