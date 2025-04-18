using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RavelDev.Discord.Bot.Entity;
using RavelDev.Discord.Bot.Entity.Models.Config;
using RavelDev.Discord.Bot.Entity.Models.Discord;
using RavelDev.Discord.Bot.Entity.Models.YouTube;


namespace RavelDev.Discord.Bot.Entity
{
    public class DiscordBotDbContext : DbContext
    {
        public DiscordBotDbContext(DbContextOptions<DiscordBotDbContext> options) : base(options)
        {

        }

        public DbSet<BotConfig> BotConfigs { get; set; }
        public DbSet<BotAuthorizedUser> BotAuthorizedUsers { get; set; }
        public DbSet<DiscordUser> DiscordUsers { get; set; }
        public DbSet<DiscordUserDisplayName> DiscordUserDisplayNames { get; set; }
        public DbSet<YouTubePlay> YouTubePlays { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<YouTubePlaySource> YouTubePlaySources { get; set; }

    }

}

public class DiscordBotDbContextFactory :
        IDesignTimeDbContextFactory<DiscordBotDbContext>
{
    public DiscordBotDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<DiscordBotDbContextFactory>()
            .Build();

        var keyVaultName = configuration.GetSection("AzureKeys:KeyVaultName").Get<string>();

        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        var kvClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        var connectionStringSecret = kvClient.GetSecret("local-db-cs").Value.Value;

        var builder = new DbContextOptionsBuilder<DiscordBotDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.UseSqlServer(connectionString);
        return new DiscordBotDbContext(builder.Options);
    }

}
