using Discord;
using Discord.WebSocket;
using RavelDev.Discord.Bot.Core.API;
using RavelDev.Discord.Bot.Core.Commands;
using RavelDev.Discord.Bot.Core.Models;

namespace RavelDev.Discord.Bot.Core
{
    public class MauriceBot
    {
        List<CustomCommand> customCommands = new List<CustomCommand>();
        public required string DiscordToken { get; set; }
        private DiscordSocketClient DiscordClient { get; }
        public ulong ClientId { get; private set; }
        public DiscordServerConfigApi ConfigApi { get; }
        Dictionary<ulong, BotConfigModel> ServerConfigs { get; set; } = new Dictionary<ulong, BotConfigModel>();

        private async Task Connected()
        {
            this.ClientId = DiscordClient.CurrentUser.Id;
            foreach(var guild in DiscordClient.Guilds)
            {
                if (!ServerConfigs.ContainsKey(guild.Id))
                {
                    try
                    {
                        var config = ConfigApi.GetConfigForServer(guild.Id);
                        if (config != null)
                            ServerConfigs[guild.Id] = config;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error fetching config for guild {guild.Name}, ID: {guild.Id}", ex); 
                    }

                }
            }
            foreach (var command in customCommands)
            {
                command.ClientId = this.ClientId;
            }
        }

        public MauriceBot(
            DiscordServerConfigApi configApi,
            List<CustomCommand> commands)
        {
            ConfigApi = configApi;
            var config = new DiscordSocketConfig()
            {
                GatewayIntents =  GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            this.DiscordClient = new DiscordSocketClient(config);
            this.customCommands = commands;

            foreach (var command in customCommands)
            {
                command.Init();
            }
        }

        public async Task Connect()
        {
            try
            {
                DiscordClient.Log += Log;

                await DiscordClient.LoginAsync(TokenType.Bot, DiscordToken);
                await DiscordClient.StartAsync();
                DiscordClient.MessageReceived += MessageReceived;
                DiscordClient.Connected += Connected;

                // Block this task until the program is closed.
                await Task.Delay(-1);

                Task Log(LogMessage msg)
                {
                    Console.WriteLine(msg.ToString());
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            try
            {
                var usermsg = msg as SocketUserMessage;
                if (usermsg == null) return;
               
                var author = usermsg.Author as SocketGuildUser;
                if (author == null) return;

                var parseForCommand = true;
                if (ServerConfigs.ContainsKey(author.Guild.Id))
                {
                    var config = ServerConfigs[author.Guild.Id];
                    if (config.BotConfiJsong.AuthorizedUsersOnly)
                    {
                        parseForCommand = config.AuthorizedUsers?.Any(user => user.DiscordUserId == author.Id) ?? false;
                    }
                    
                }

                var msgText = usermsg.Content;

                if (string.IsNullOrEmpty(msgText)) return;
                if (parseForCommand)

                {
                    foreach (var command in customCommands)
                    {
                        if (command.IsCommand(msgText))
                        {
                            await command.ExecuteAsync(usermsg, author);
                            break;
                        }
                    }
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }

        }
    }
}