using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agora.DAOs.Persistence.Migrations
{
    public partial class AddConcludedDateToHearing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConcludedDate",
                table: "Hearings",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcludedDate",
                table: "Hearings");
        }
    }
}
