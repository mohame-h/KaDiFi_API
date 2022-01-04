using Microsoft.EntityFrameworkCore.Migrations;

namespace KaDiFi.Migrations
{
    public partial class removeviewscountproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewsCount",
                table: "Media");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewsCount",
                table: "Media",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
