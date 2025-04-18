using System.ComponentModel.DataAnnotations.Schema;

namespace RavelDev.Discord.Bot.Entity.Models.Config
{
    public class BotConfig
    {
        public int BotConfigId { get; set; }

        [Column(TypeName = "bigint")]
        public long GuldId { get; set; }
        public string BotConfigJson { get; set; }
    }
}
