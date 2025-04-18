using RavelDev.Discord.Bot.Core.DataAccess;


namespace RavelDev.Discord.Bot.Core.API
{
    public class DiscordUserApi
    {
        public DiscordUserApi(DiscordUserRepository userRepo)
        {
            UserRepo = userRepo;
        }
        private DiscordUserRepository UserRepo { get; set; }


        internal void RecordDisordUserData(ulong guildId, string serverName,
            ulong userId,
            string userDisplayName, string username)
        {
                UserRepo.InsertDiscordGuild(guildId, serverName);
                UserRepo.InsertDiscordUser(userId, userDisplayName, username);
        }
    }
}
