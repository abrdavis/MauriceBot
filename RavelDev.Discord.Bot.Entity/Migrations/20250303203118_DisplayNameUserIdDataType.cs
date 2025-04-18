using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavelDev.Discord.Bot.Entity.Migrations
{
    /// <inheritdoc />
    public partial class DisplayNameUserIdDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DiscordUserId",
                table: "DiscordUserDisplayNames",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscordUserId",
                table: "DiscordUserDisplayNames",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
