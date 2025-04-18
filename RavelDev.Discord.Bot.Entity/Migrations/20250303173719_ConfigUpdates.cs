using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavelDev.Discord.Bot.Entity.Migrations
{
    /// <inheritdoc />
    public partial class ConfigUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotAuthorizedUsers",
                columns: table => new
                {
                    BotAuthorizedUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    DiscordUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotAuthorizedUsers", x => x.BotAuthorizedUserId);
                });

            migrationBuilder.CreateTable(
                name: "BotConfigs",
                columns: table => new
                {
                    BotConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuldId = table.Column<long>(type: "bigint", nullable: false),
                    BotConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotConfigs", x => x.BotConfigId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotAuthorizedUsers");

            migrationBuilder.DropTable(
                name: "BotConfigs");
        }
    }
}
