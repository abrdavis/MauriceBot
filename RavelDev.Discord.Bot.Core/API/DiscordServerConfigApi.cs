using Newtonsoft.Json;
using RavelDev.Discord.Bot.Core.DataAccess;
using RavelDev.Discord.Bot.Core.Models;

namespace RavelDev.Discord.Bot.Core.API
{
    public class DiscordServerConfigApi
    {
        private ServerConfigRepository BotRepository { get; set; }
        public DiscordServerConfigApi(ServerConfigRepository botRepository)
        {
            BotRepository = botRepository;
        }

        public BotConfigModel? GetConfigForServer(ulong guildId)
        {
            BotConfigModel? result = null;
            var configModel = new BotConfigJson();
            var configJsonStr = BotRepository.GetConfigStringForServer(guildId);
            if (!string.IsNullOrEmpty(configJsonStr))
            {
                configModel = JsonConvert.DeserializeObject<BotConfigJson>(configJsonStr);
            }

            var authedUsers = BotRepository.GetAuthorizedUsersForServer(guildId);
            result = new BotConfigModel(configModel, authedUsers);
            return result;
        }
    }
}
