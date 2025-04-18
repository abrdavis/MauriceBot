using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Models
{
    public class BotConfigModel
    {
        public BotConfigJson BotConfiJsong { get; set; }
        public List<DiscordUserModel> AuthorizedUsers { get; set; }

        public BotConfigModel(BotConfigJson botConfigJson, List<DiscordUserModel> authorizedUsers)
        {
            BotConfiJsong = botConfigJson;
            AuthorizedUsers = authorizedUsers;
        }
    }
}
