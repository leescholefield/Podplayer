using Microsoft.EntityFrameworkCore.Migrations;

namespace Podplayer.Entity.Migrations
{
    public partial class NumberSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberSubscriptions",
                table: "Podcasts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberSubscriptions",
                table: "Podcasts");
        }
    }
}
