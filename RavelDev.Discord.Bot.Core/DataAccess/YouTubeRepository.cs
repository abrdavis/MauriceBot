using Dapper;
using Microsoft.Data.SqlClient;
using RavelDev.Discord.Bot.Core.Utility.Interfaces;
using System.Data;


namespace RavelDev.Discord.Bot.Core.DataAccess
{
    public class YouTubeRepository
    {
        public YouTubeRepository(IRepositoryConfig repoConfig)
        {
            RepoConfig = repoConfig;
        }

        public IRepositoryConfig RepoConfig { get; private set; }

        public void InsertYouTubePlay(string title, 
            decimal userId, 
            decimal guildId,
            string videoId, 
            int playSourceId, 
            string sourceUrl)
        {
            using (var conn = new SqlConnection(RepoConfig.ConnectionString))
            {
                const string sProcName = "[YouTubePlayInsert]";
                var values = new
                {
                    title = title,
                    userId = userId,
                    guildId = guildId,
                    videoId = videoId,
                    sourceUrl = sourceUrl,
                    playSourceId = playSourceId,
                };

                conn.Execute(sProcName,
                            values,
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 600);
            }
        }
    }
}
