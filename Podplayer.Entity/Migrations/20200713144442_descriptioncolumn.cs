using Microsoft.EntityFrameworkCore.Migrations;

namespace Podplayer.Entity.Migrations
{
    public partial class descriptioncolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Podcasts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Podcasts");
        }
    }
}
