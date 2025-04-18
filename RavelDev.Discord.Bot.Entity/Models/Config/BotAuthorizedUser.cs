using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Entity.Models.Config
{
    public class BotAuthorizedUser
    {
        public int BotAuthorizedUserId { get; set; }
        public ulong GuildId { get; set; }
        public ulong DiscordUserId { get; set; }
    }
}
