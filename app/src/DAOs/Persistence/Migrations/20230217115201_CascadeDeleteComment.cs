using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class CascadeDeleteComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentParrentId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentParrentId",
                table: "Comments",
                column: "CommentParrentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentParrentId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentParrentId",
                table: "Comments",
                column: "CommentParrentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
