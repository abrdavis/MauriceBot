using Dapper;
using Microsoft.Data.SqlClient;
using RavelDev.Discord.Bot.Core.Models;
using RavelDev.Discord.Bot.Core.Utility.Interfaces;
using System.Data;

namespace RavelDev.Discord.Bot.Core.DataAccess
{
    public class ServerConfigRepository
    {
        public IRepositoryConfig RepoConfig { get; }

        public ServerConfigRepository(IRepositoryConfig config)
        {
            this.RepoConfig = config;
        }

        public List<DiscordUserModel> GetAuthorizedUsersForServer(decimal guildId)
        {
            var result = new List<DiscordUserModel>();
            using (var conn = new SqlConnection(RepoConfig.ConnectionString))
            {
                const string sProcName = "[GetAuthorizedBotUsersForGuild]";
                var values = new
                {   
                    guildId = guildId,
                };

                result = (conn.Query<DiscordUserModel>(sProcName,
                            values,
                            commandType: CommandType.StoredProcedure))?.ToList() ?? result;

             }

            return result;
        }

        internal string GetConfigStringForServer(decimal guildId)
        {
            var result = string.Empty;
            using (var conn = new SqlConnection(RepoConfig.ConnectionString))
            {
                const string sProcName = "[BotConfigGetConfigJsonForServer]";
                var values = new
                {
                    guildId = guildId,
                };

                result =  (conn.Query<string>(sProcName,
                            values,
                            commandType: CommandType.StoredProcedure)).FirstOrDefault();

            }

            return result;
        }
    }
}
