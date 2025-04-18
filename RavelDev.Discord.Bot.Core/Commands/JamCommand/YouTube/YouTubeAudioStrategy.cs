namespace RavelDev.Discord.Bot.Core.Commands.JamCommand.YouTube
{
    public abstract class YouTubeAudioStrategy
    {
        public abstract Task<YouTubeAudioResponse> GetYouTubeAudioAsync(string youTubeUrl);
    }

    public class YouTubeAudioResponse
    {
        public YouTubeAudioResponse() {
            VideoName = string.Empty;
            VideoId = string.Empty;
        }

        public YouTubeAudioResponse(Stream audioStream, string videoName, string videoId)
        {
            AudioStream = audioStream;
            DataInStream = true;
            VideoName = videoName;
            VideoId = videoId;
        }

        public YouTubeAudioResponse(Stream audioSteam, string audioFilePath, string videoName, string videoId)
        {
            AudioStream = audioSteam;
            AudioFilePath = audioFilePath;
            VideoName = videoName;
            VideoId = videoId;
        }

        public Stream? AudioStream { get; set; }
        public string? AudioFilePath { get; set; }
        public bool DataInStream { get; set; }
        public string VideoName { get; set; }
        public string VideoId { get; set; }
    }
}
