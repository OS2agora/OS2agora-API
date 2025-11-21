using Microsoft.EntityFrameworkCore.Migrations;

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class CascadeDeleteContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Comments_CommentId",
                table: "Contents");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Comments_CommentId",
                table: "Contents",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Comments_CommentId",
                table: "Contents");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Comments_CommentId",
                table: "Contents",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
