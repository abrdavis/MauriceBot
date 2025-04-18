using System.ComponentModel.DataAnnotations.Schema;


namespace RavelDev.Discord.Bot.Entity.Models.Discord
{
    public class DiscordUserDisplayName
    {
        public int DiscordUserDisplayNameId { get; set; }

        [Column(TypeName = "bigint")]
        public ulong DiscordUserId { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateSeen { get; set; }
    }
}
