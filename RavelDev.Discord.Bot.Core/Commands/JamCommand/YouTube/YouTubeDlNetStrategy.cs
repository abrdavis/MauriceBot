using Discord.Audio.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Options;
using YoutubeDLSharp;

namespace RavelDev.Discord.Bot.Core.Commands.JamCommand.YouTube
{
    public class YouTubeDlNetStrategy : YouTubeAudioStrategy
    {
        //--cookies-from-browser chrome
        public YouTubeDlNetStrategy() { }
        public override async Task<YouTubeAudioResponse> GetYouTubeAudioAsync(string youTubeUrl)
        {
            var ytdl = new YoutubeDL();
            var vidData = await ytdl.RunVideoDataFetch(youTubeUrl);
            var res = await ytdl.RunAudioDownload(
                            youTubeUrl,
                AudioConversionFormat.Best);
            var filePath = res.Data;
            var fileStream =  File.OpenRead(res.Data);
            return new YouTubeAudioResponse(fileStream, filePath, videoId: vidData.Data.ID, 
                videoName: vidData.Data.Title);
        }
    }
}
