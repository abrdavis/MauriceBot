using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Entity.Models.Discord
{
    public class DiscordUser
    {
        [Column(TypeName = "bigint")]
        public ulong DiscordUserId { get; set; }
        public required string UserName { get; set; }
        public required string DisplayName { get; set; }
    }
}
