using Microsoft.EntityFrameworkCore.Migrations;

namespace KaDiFi.Migrations
{
    public partial class removemediaName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Media");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
