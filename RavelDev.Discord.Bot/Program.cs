using dotenv.net;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using RavelDev.Discord.Bot.Core;
using RavelDev.Discord.Bot.Core.API;
using RavelDev.Discord.Bot.Core.Commands.GoogleAi;
using RavelDev.Discord.Bot.Core.Commands;
using RavelDev.Discord.Bot.Core.DataAccess;
using RavelDev.Discord.Bot.Core.Utility.Interfaces;
using RavelDev.Spotify;
using YoutubeDLSharp;
using RavelDev.Discord.Bot.Core.Commands.YouTube;
using RavelDev.Discord.Bot.Core.Commands.JamCommand.YouTube;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

IDictionary<string, string> envVars;
envVars = DotEnv.Read();

var keyVaultName = envVars["KEY_VAULT_NAME"];
var kvUri = $"https://{keyVaultName}.vault.azure.net";

var kvClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

var connectionStringSecret = await kvClient.GetSecretAsync("local-db-cs");

var connectionString = connectionStringSecret.Value.Value;
if (connectionString == null)
{
    Console.WriteLine("Connection string not configured.");
    return;
}



var googleAiApiKey = envVars["GOOGLE_AI_API"];

int maxDiscordMsgLength = 2000;
int.TryParse(envVars["MAX_DISCORD_LENGTH"], out maxDiscordMsgLength);
var discordtoken = envVars["DISCORD_TOKEN"];
var ffMpegYtAudioCmdArguments = envVars["FFMPEG_YT_AUDIO_ENCODE"];
var botName = envVars["BOT_NAME"];
var ytApiKey = envVars["YT_API"];
var spotifyClientId = envVars["SPOTIFY_CLIENT_ID"];
var spotifySecret = envVars["SPOTIFY_SECRET"];

var repoConfig = new RepositoryConfig { ConnectionString = connectionString };

var YouTubeRepo = new YouTubeRepository(repoConfig);
var discordBotRepo = new ServerConfigRepository(repoConfig);
var discordConfigApi = new DiscordServerConfigApi(discordBotRepo);
var SpotifyApi = new SpotifyWebApi() { SpotifyClientId = spotifyClientId, SpotifySecret = spotifySecret };
var DiscordUserRepo = new DiscordUserRepository(repoConfig);
var DisordUserApi = new DiscordUserApi(DiscordUserRepo);
var youtubeService = new YouTubeService(new BaseClientService.Initializer()
{
    ApiKey = ytApiKey,
    ApplicationName = "MauriceBot"
});
var YouTubeApi = new YouTubeApi(youtubeService);
var spotifyMapper = new SpotifyYouTubeMapper(youtubeService, SpotifyApi);
var ytAudioStratgies = new List<YouTubeAudioStrategy>()
        {

            new YouTubeExplodeStrategy(),
            new YouTubeDlNetStrategy()
        };


var customCommands = new List<CustomCommand> {
                new JamCommand(
                    YouTubeRepo,
                    YouTubeApi,
                    spotifyMapper,
                    new YoutubeDL(),
                    DisordUserApi,
                    ytAudioStratgies) {
                    CommandPrefix = "jam", 
                    MaxDiscordMessageLength = maxDiscordMsgLength,
                    FfmpegCommandLineCommand =  ffMpegYtAudioCmdArguments
                },
                new GoogleAiCommand(new HttpClient()) {
                    ApiKey = googleAiApiKey, 
                    CommandPrefix = botName, 
                    MaxDiscordMessageLength = maxDiscordMsgLength},
            };

var bot = new MauriceBot(discordConfigApi, customCommands) { DiscordToken=discordtoken};
await bot.Connect();
