using Dapper;
using Microsoft.Data.SqlClient;
using RavelDev.Discord.Bot.Core.Utility.Interfaces;
using System.Data;

namespace RavelDev.Discord.Bot.Core.DataAccess
{
    public class DiscordUserRepository
    {
        public DiscordUserRepository(IRepositoryConfig repoConfig) {
            RepoConfig = repoConfig;
        }

        public IRepositoryConfig RepoConfig { get; }

        public void InsertDiscordGuild(decimal guildId, string guildName)
        {

            using (var conn = new SqlConnection(RepoConfig.ConnectionString))
            {
                const string sProcName = "[DiscordGuildInsert]";
                var values = new
                {
                    guildId = guildId,
                    guildName = guildName,
                };

                conn.Execute(sProcName,
                            values,
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 600);
            }


        }

        internal void InsertDiscordUser(decimal discordUserId, string displayName, string userName)
        {
            using (var conn = new SqlConnection(RepoConfig.ConnectionString))
            {
                const string sProcName = "[DiscordUserInsert]";
                var values = new
                {
                    userId = discordUserId,
                    userName = userName,
                    displayName = displayName
                };

                conn.Execute(sProcName,
                            values,
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 600);
            }
        }
    }
}
