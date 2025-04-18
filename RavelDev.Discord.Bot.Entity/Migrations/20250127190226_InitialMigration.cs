using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavelDev.Discord.Bot.Entity.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUserDisplayNames",
                columns: table => new
                {
                    DiscordUserDisplayNameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSeen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUserDisplayNames", x => x.DiscordUserDisplayNameId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    DiscordUserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UQ_DiscordUsers", x => x.DiscordUserId);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    GuildName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UQ_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "YouTubePlays",
                columns: table => new
                {
                    YouTubePlayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YouTubeVideoId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscordUserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    YouTubePlaySourceId = table.Column<int>(type: "int", nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YouTubePlays", x => x.YouTubePlayId);
                });

            migrationBuilder.CreateTable(
                name: "YouTubePlaySources",
                columns: table => new
                {
                    YouTubePlaySourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YouTubePlaySources", x => x.YouTubePlaySourceId);
                });

            migrationBuilder.Sql(@"SET IDENTITY_INSERT YouTubePlaySources ON
                    INSERT INTO YouTubePlaySources(YouTubePlaySourceId, SourceName) VALUES (1, 'YouTube'), (2, 'Spotify')
SET IDENTITY_INSERT YouTubePlaySources OFF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordUserDisplayNames");

            migrationBuilder.DropTable(
                name: "DiscordUsers");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "YouTubePlays");

            migrationBuilder.DropTable(
                name: "YouTubePlaySources");
        }
    }
}
