using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KaDiFi.Migrations
{
    public partial class modifingviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ViewTime",
                table: "MediaViews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewTime",
                table: "MediaViews");
        }
    }
}
