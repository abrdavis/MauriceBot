using AngleSharp.Dom;
using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Audio.Streams;
using Discord.Commands;
using Discord.WebSocket;
using RavelDev.Discord.Bot.Core.Models.YouTube;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace RavelDev.Discord.Bot.Core.Commands
{

    public class YouTubeModule : ModuleBase<SocketCommandContext>
    {

        private bool IsPlaying { get; set; }
        public Queue<YouTubeQueueItem> PlayQueue = new Queue<YouTubeQueueItem>();
        private async Task StreamVideo(YouTubeQueueItem queueItem)
        {
            IsPlaying = true;
            YoutubeClient youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(queueItem.VideoUrl);

            var audioStreamInfo = streamManifest
                                .GetAudioOnlyStreams()
                                .GetWithHighestBitrate();
            var InputStream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);


            IAudioClient audioClient = null;
            try
            {
                audioClient = await queueItem.VoiceChannel.ConnectAsync();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            var Memory = new MemoryStream();
            try
            {
                await Cli.Wrap("ffmpeg").WithArguments(" -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1"
    ).WithStandardInputPipe(PipeSource.FromStream(InputStream))
    .WithStandardOutputPipe(PipeTarget.ToStream(Memory)).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            using (AudioOutStream OutputStream = audioClient!.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    var mem = Memory.ToArray();
                    await OutputStream.WriteAsync(mem, 0, (int)Memory.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally { await OutputStream.FlushAsync(); }
            }

            if (PlayQueue.Any()){
                StreamVideo(PlayQueue.Dequeue());
            }
            else
            {
                IsPlaying = false;
            }
        }
        [Command("yt", RunMode = RunMode.Async)]
        [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("yt")]
        public async Task UserInfoAsync(
            [Summary("The (optional) user to get info from")]
        string videoUrl)
        {

            var user = Context.User as SocketGuildUser;
            var channel = user!.VoiceChannel;

    
            if (!IsPlaying)
            {
                StreamVideo(PlayQueue.Dequeue());
            }

        }



    }
}
