using Discord.Audio.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace RavelDev.Discord.Bot.Core.Commands.JamCommand.YouTube
{
    public class YouTubeExplodeStrategy : YouTubeAudioStrategy
    {
        public YouTubeExplodeStrategy()
        {
        }

        public async override Task<YouTubeAudioResponse> GetYouTubeAudioAsync(string youTubeUrl)
        {
            var ytExplode = new YoutubeClient();
            var vidInfo = await ytExplode.Videos.GetAsync(youTubeUrl);
            var streamManifest = await ytExplode.Videos.Streams.GetManifestAsync(youTubeUrl);
            var audioStreamInfo = streamManifest
            .GetAudioOnlyStreams()
            .GetWithHighestBitrate();
            var audioStream = await ytExplode.Videos.Streams.GetAsync(audioStreamInfo);
            return new YouTubeAudioResponse(audioStream, vidInfo.Title, vidInfo.Id);
        }
    }
}
