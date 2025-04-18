namespace RavelDev.Discord.Bot.Core.Utility.Interfaces
{
    public interface IRepositoryConfig
    {
        string ConnectionString { get; }
    }

    public class RepositoryConfig : IRepositoryConfig
    {
        public RepositoryConfig()
        {
        }
        public required string ConnectionString { get; set; }
    }
}
