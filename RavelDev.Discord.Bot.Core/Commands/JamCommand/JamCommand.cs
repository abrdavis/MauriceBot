using CliWrap;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using RavelDev.Discord.Bot.Core.API;
using RavelDev.Discord.Bot.Core.Commands.JamCommand.YouTube;
using RavelDev.Discord.Bot.Core.DataAccess;
using RavelDev.Discord.Bot.Core.Models.YouTube;
using RavelDev.Discord.Bot.Core.Utility;
using System.Text.RegularExpressions;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeExplode.Common;


namespace RavelDev.Discord.Bot.Core.Commands.YouTube
{
    public class JamCommandArguments
    {
        public static string Stop = "stop";
        public static string Skip = "skip";
    }
    public class JamCommand : CustomCommand
    {
        public required string FfmpegCommandLineCommand { get;  set; }
        private YouTubeRepository YouTubeRepo { get; }
        private YoutubeDL YtDl { get; }
        private DiscordUserApi DiscordUserApi { get; }

        private YouTubeApi YouTubeApi { get; }
        private SpotifyYouTubeMapper SpotifyYouTubeMapper { get; }

        private List<string> ValidArguments = new List<string>() { JamCommandArguments.Skip, JamCommandArguments.Stop };

        private Regex SpotifyUrlRegEx = new Regex(RegExPatterns.SpotifyTrackUrl, RegexOptions.IgnoreCase);
        private Regex YtRegex = new Regex(RegExPatterns.YouTubeVideoUrl, RegexOptions.IgnoreCase);
        private Regex YtPlaylistRegeEx = new Regex(RegExPatterns.YouTubePlaylistUrl, RegexOptions.IgnoreCase);

        private Dictionary<ulong, YouTubeStreamInstance> ActiveStreams = new Dictionary<ulong, YouTubeStreamInstance>();
        private Dictionary<ulong, IAudioClient> AudioClients = new Dictionary<ulong, IAudioClient>();
        private Dictionary<ulong, YouTubeQueueItem> CurrentPlaying = new Dictionary<ulong, YouTubeQueueItem>();
        private Dictionary<ulong, Queue<YouTubeQueueItem>> PlayQueue = new Dictionary<ulong, Queue<YouTubeQueueItem>>();
        private List<YouTubeAudioStrategy> YouTubeStartegies { get; set; }
        private Dictionary<ulong, bool> PlayingStatus { get; set; } = new Dictionary<ulong, bool>();

        public JamCommand(
            YouTubeRepository repo,
            YouTubeApi ytApi,
            SpotifyYouTubeMapper spotifyMapper,
            YoutubeDL ytDl,
            DiscordUserApi discordUserApi,
            List<YouTubeAudioStrategy> ytAudioStrategies) 
        {
            YouTubeRepo = repo;
            YouTubeApi = ytApi;
            SpotifyYouTubeMapper = spotifyMapper;
            YtDl = ytDl;
            DiscordUserApi = discordUserApi;
            YouTubeStartegies = ytAudioStrategies;
        }

        public async override Task ExecuteAsync(SocketUserMessage msg, SocketGuildUser user)
        {
            var messageText = msg.Content;
            var voiceChannel = user.VoiceChannel;

            if (voiceChannel == null) return;

            var validArgument = false;

            var guildId = user.Guild.Id;
            var serverName = user.Guild.Name;

            try
            {
                DiscordUserApi.RecordDisordUserData(guildId, serverName, user.Id, user.DisplayName, user.Username);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error recording discord user data", ex);
                return;
            }
            

            var msgSplit = messageText.Split($"!{CommandPrefix}", StringSplitOptions.None);
            var msgChannel = msg.Channel;

            var commandArgument = msgSplit[1];

            var playerCommand = false;

            var isYouTubeVideo = YtRegex.IsMatch(commandArgument);
            if (isYouTubeVideo)
            {
                await AddQueueItem(user, voiceChannel,
                    guildId, msgChannel,
                    youtubeUrl: commandArgument,
                    sourceUrl: commandArgument,
                    queueSourceId: SongRequestSource.YouTube);
                ProessMusicQueue(guildId);
                return;
            }


            var isSpotifyTrack = SpotifyUrlRegEx.IsMatch(commandArgument);
            if (isSpotifyTrack)
            {
                var spotifyTrackId = SpotifyUrlRegEx.Matches(commandArgument).FirstOrDefault();
                YouTubeMap? ytMap = null;
                try
                {
                    ytMap = await SpotifyYouTubeMapper.GetYouTubeDataForSpotifyTrack(spotifyTrackId?.Groups[1]?.Value!);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error retrieving spotify track data", ex);
                    return;
                }
                if (ytMap != null)
                {
                    await AddQueueItem(user,
                        voiceChannel,
                        guildId,
                        msgChannel,
                        youtubeUrl: ytMap?.YouTubeUrl!,
                        sourceUrl: commandArgument,
                        displayText: ytMap?.DisplayName!,
                        albumArtUrl: ytMap!.AlbumUrl,
                        queueSourceId: SongRequestSource.Spotify);
                    ProessMusicQueue(guildId);
                    return;
                }
            }
            

            var isYouTubePlaylist = YtPlaylistRegeEx.IsMatch(commandArgument);
            if (isYouTubePlaylist)
            {
                var matches = YtPlaylistRegeEx.Matches(commandArgument).FirstOrDefault();
                var playlistId = matches?.Groups[2]?.Value!;
                var ytData = await YouTubeApi.GetPlaylistData(playlistId);
                if (!ytData.Any())
                {
                    await msgChannel.SendMessageAsync($"No data found for playlist");
                    return;
                }

                foreach (var vidInfo in ytData)
                {
                    await AddQueueItem(user, voiceChannel, guildId, msgChannel,
                        vidInfo.YouTubeUrl, vidInfo.YouTubeUrl,
                        queueSourceId: SongRequestSource.Spotify,
                        displayText: vidInfo.DisplayName ?? string.Empty,
                        displayMessage: false);
                }
                await msgChannel.SendMessageAsync($"{ytData.Count} items from playlist added to queue.");
                ProessMusicQueue(guildId);
                return;
            }
            

            if (!validArgument && ValidArguments.IndexOf(commandArgument.Trim()) >= 0)
            {
                validArgument = playerCommand = true;
            }

            if (!validArgument)
            {
                await msg.Channel.SendMessageAsync($"I just don't understand what to do with this, yo.");
                return;
            }
            
            if (playerCommand)
            {
                var argLowercase = commandArgument.ToLower().Trim();

                if (argLowercase == JamCommandArguments.Skip)
                {
                    if (ActiveStreams.ContainsKey(guildId))
                    {
                        if (CurrentPlaying.ContainsKey(guildId))
                        {
                            var currentItem = CurrentPlaying[guildId];
                            var footerText = $"Requested by {user.DisplayName}";
                            var builder = GetBuilderWithDescription($"Skipping [{currentItem.DisplayText}]({currentItem.SourceUrl}).",
                                Color.Red,
                                footerText,
                                user.GetDisplayAvatarUrl());
                            await msgChannel.SendMessageAsync("", false, builder.Build());

                        }

                        ActiveStreams[guildId].Cancel();

                        if (PlayQueue[guildId].Any())
                        {
                            PlayYouTubeAudio(PlayQueue[guildId].Dequeue());
                        }
                        else
                        {
                            PlayingStatus[guildId] = false;
                            await ActiveStreams[guildId].LeaveChannel();
                        }
                    }

                    return;
                }
                else if (argLowercase == JamCommandArguments.Stop)
                {
                    if (ActiveStreams.ContainsKey(guildId))
                    {
                        ActiveStreams[guildId].Cancel();
                        PlayingStatus[guildId] = false;
                        await ActiveStreams[guildId].LeaveChannel();
                        PlayQueue[guildId].Clear();
                    }
                    var footerText = $"Requested by {user.DisplayName}";
                    var builder = GetBuilderWithDescription("Playback cancelled and queue cleared.",
                        Color.Red,
                        footerText,
                        user.GetDisplayAvatarUrl());
                    await msgChannel.SendMessageAsync("", false, builder.Build());
                    return;
                }
            }

            return;
        }

        private static EmbedBuilder GetBuilderWithDescription(
            string descriptionText,
            Color color,
            string footerText = "",
            string iconUrl = "")
        {
            var builder = new EmbedBuilder()
                .WithColor(color)
                .WithDescription(descriptionText);
            if (!string.IsNullOrWhiteSpace(footerText))
            {
                builder.WithFooter(f => { f.WithText(footerText).WithIconUrl(iconUrl); });
            }

            return builder;
        }

        private async Task AddQueueItem(SocketGuildUser user,
            SocketVoiceChannel voiceChannel,
            ulong guildId,
            ISocketMessageChannel msgChannel,
            string youtubeUrl,
            string sourceUrl,
            int queueSourceId,
            string displayText = "",
            string albumArtUrl = "",
            bool displayMessage = true)
        {

           RunResult<VideoData>? vidInfo = null;

            YouTubeQueueItem queueItem;
            if (queueSourceId == SongRequestSource.YouTube)
            {
                try
                {
                    vidInfo = await YtDl.RunVideoDataFetch(youtubeUrl);
                    if(vidInfo.Data != null)
                    {
                        displayText = $"{vidInfo.Data.Title}";
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            queueItem = new YouTubeQueueItem(youtubeUrl,
                            sourceUrl,
                            queueSourceId,
                            voiceChannel,
                            msgChannel,
                            guildId,
                            user.Id,
                            displayText,
                            user.DisplayName,
                            user.GetDisplayAvatarUrl(size: 64));

            if (!PlayQueue.ContainsKey(guildId))
            {
                PlayQueue[guildId] = new Queue<YouTubeQueueItem>();
            }

            if (CurrentlyPlayingForServer(guildId))
            {
                var currentlyPlayingTxt = GetCurrentlyPlayingText(guildId);

                int position = PlayQueue[guildId].Count + 1;
                var queueText = GetQueueText(guildId);

                var fields = new List<EmbedBuilder>();

                var builder = new EmbedBuilder()
                    .WithColor(Color.Teal)
                    .WithDescription($"***{queueItem.DisplayText}*** added to queue position {position}.")
                    .AddField("Currently Playing", currentlyPlayingTxt, true)
                    .AddField("Songs Queued", PlayQueue[guildId].Count + 1, true)
                    .WithFooter(f => f.WithText($"Requested by {queueItem.RequestingUserName}")
                    .WithIconUrl(queueItem.UserProfileImage));

                if (!string.IsNullOrEmpty(albumArtUrl))
                {
                    builder.WithThumbnailUrl(albumArtUrl);
                }

                if (displayMessage)
                    await queueItem.TextChannel.SendMessageAsync("", false, builder.Build());
            }

            PlayQueue[guildId].Enqueue(queueItem);
        }

        private string GetCurrentlyPlayingText(ulong guildId)
        {
            var currentlyPlaying = string.Empty;
            if (CurrentPlaying.ContainsKey(guildId))
            {
                currentlyPlaying = CurrentPlaying[guildId].DisplayText;
            }

            return currentlyPlaying;
        }

        private string GetQueueText(ulong guildId)
        {
            if (!CurrentPlaying.ContainsKey(guildId))
            {
                return string.Empty;
            }
            var currentlyPlaying = CurrentPlaying[guildId];

            return $"***Songs in Queue***: {PlayQueue[guildId].Count + 1}  ***Currently Playing***: {currentlyPlaying.DisplayText}";
        }

        private bool CurrentlyPlayingForServer(ulong guildId)
        {
            return PlayingStatus.ContainsKey(guildId) && PlayingStatus[guildId];
        }

        private void ProessMusicQueue(ulong guildId)
        {
            if (!PlayingStatus.ContainsKey(guildId))
            {
                PlayingStatus[guildId] = false;
            }

            if (PlayingStatus[guildId] == false)
            {
                PlayYouTubeAudio(PlayQueue[guildId].Dequeue());
            }
        }

        private async void PlayYouTubeAudio(YouTubeQueueItem queueItem)
        {
            var operationCancelled = false;
            await Task.Run(async () =>
            {

                var filePath = string.Empty;
                var ytResponse = new YouTubeAudioResponse();
                try
                {
                    CurrentPlaying[queueItem.GuildId] = queueItem;
                    PlayingStatus[queueItem.GuildId] = true;
                    foreach(var strategy in YouTubeStartegies)
                    {
                        try
                        {
                            ytResponse = await strategy.GetYouTubeAudioAsync(queueItem.VideoUrl);
                            if (ytResponse != null) break;
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
 
                    if(ytResponse == null)
                    {
                        Console.WriteLine("Unable to retrieve youtube audio data.");
                        return;

                    }
                    var builder = new EmbedBuilder()
                    .WithColor(Color.Teal)
                    .WithDescription($"Now Playing [{queueItem.DisplayText}]({queueItem.SourceUrl}).")
                    .WithFooter(f => f.WithText($"Requested by {queueItem.RequestingUserName}")
                    .WithIconUrl(queueItem.UserProfileImage));

                    if (PlayQueue[queueItem.GuildId].Any())
                    {
                        YouTubeQueueItem? upNext;
                        PlayQueue[queueItem.GuildId].TryPeek(out upNext);
                        if (upNext != null)
                        {
                            builder.AddField("Up Next", upNext.DisplayText, true);
                            builder.AddField("Songs Queued", PlayQueue[queueItem.GuildId].Count, true);
                        }
                    }
                    var embed = builder.Build();
                    await queueItem.TextChannel.SendMessageAsync("", false, embed);

                    try
                    {
                         YouTubeRepo.InsertYouTubePlay(title: ytResponse.VideoName,
                             userId: queueItem.DiscordUserId,
                             guildId: queueItem.GuildId,
                             videoId: ytResponse.VideoId,
                             playSourceId: queueItem.PlaySourceId,
                             sourceUrl: queueItem.SourceUrl);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error logging youtube playback in DB. \n {ex.Message}");
                    }

                }

                catch (Exception ex)
                {
                    await queueItem.TextChannel.SendMessageAsync($"Error playing queue item. Moving to next track.");
                    Console.WriteLine(ex);
                }

                if (ytResponse.AudioStream == null) return;

                try
                {
                    if (!queueItem.VoiceChannel.ConnectedUsers.Any(user => user.Id == ClientId))
                        this.AudioClients[queueItem.GuildId] = await queueItem.VoiceChannel.ConnectAsync();
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
                var Memory = new MemoryStream();
                try
                {
                    await Cli.Wrap("ffmpeg").WithArguments(FfmpegCommandLineCommand)
                    .WithStandardInputPipe(PipeSource.FromStream(ytResponse.AudioStream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(Memory))
                    .ExecuteAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                if (!AudioClients.ContainsKey(queueItem.GuildId) || AudioClients[queueItem.GuildId] == null)
                    return;

                using AudioOutStream OutputStream = AudioClients[queueItem.GuildId].CreatePCMStream(AudioApplication.Mixed);

                try
                {
                    var mem = Memory.ToArray();
                    var tokenSource = new CancellationTokenSource();
                    var ct = tokenSource.Token;
                    ActiveStreams[queueItem.GuildId] = new YouTubeStreamInstance(tokenSource, queueItem.VoiceChannel);
                    Memory.Position = 0;
                    await Memory.CopyToAsync(OutputStream, tokenSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine("Playback cancelled");
                    operationCancelled = true;
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {

                    await OutputStream.FlushAsync();
                    if(ytResponse.AudioStream != null)
                        ytResponse.AudioStream.Close();

                    if (!ytResponse.DataInStream && File.Exists(ytResponse.AudioFilePath))
                    {
                        File.Delete(filePath);
                    }
                }

            });

            if (operationCancelled) return;

            if (PlayQueue[queueItem.GuildId].Any())
            {
                PlayYouTubeAudio(PlayQueue[queueItem.GuildId].Dequeue());
            }
            else
            {
                PlayingStatus[queueItem.GuildId] = false;
                await queueItem.VoiceChannel.DisconnectAsync();
            }
        }

        public override bool IsCommand(string msgText)
        {
            return msgText.ToLower().StartsWith($"!{CommandPrefix}");
        }
    }
}
