using Microsoft.EntityFrameworkCore.Migrations;

namespace Podplayer.Entity.Migrations
{
    public partial class podcastcreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Podcasts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PodcastCreators",
                columns: table => new
                {
                    CreatorId = table.Column<int>(nullable: false),
                    PodcastId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PodcastCreators", x => new { x.PodcastId, x.CreatorId });
                    table.ForeignKey(
                        name: "FK_PodcastCreators_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PodcastCreators_Podcasts_PodcastId",
                        column: x => x.PodcastId,
                        principalTable: "Podcasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PodcastCreators_CreatorId",
                table: "PodcastCreators",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PodcastCreators_PodcastId",
                table: "PodcastCreators",
                column: "PodcastId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PodcastCreators");

            migrationBuilder.DropTable(
                name: "Creators");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Podcasts");
        }
    }
}
