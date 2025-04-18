using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Models.YouTube
{
    //Queue items will be for a specific server (guild), channel, and youtube video
    public class YouTubeQueueItem
    {

        public YouTubeQueueItem(string videoUrl,
            string sourceUrl,
            int sourceId,
            SocketVoiceChannel channel,
            ISocketMessageChannel msgChannel,
            ulong guildId,
            ulong userId,
            string botDisplayText,
            string userName,
            string userProfileImage)
        {
            VideoUrl = videoUrl;
            PlaySourceId = sourceId;
            VoiceChannel = channel;
            TextChannel = msgChannel;
            GuildId = guildId;
            DiscordUserId = userId;
            DisplayText = botDisplayText;
            RequestingUserName = userName;
            UserProfileImage = userProfileImage;
            SourceUrl = sourceUrl;
        }

        public string VideoUrl { get; set; }
        public SocketVoiceChannel VoiceChannel { get; set; }
        public ISocketMessageChannel TextChannel { get; }
        public ulong GuildId { get; }
        public ulong DiscordUserId { get; set; }
        public string DisplayText { get; }
        public string SourceUrl { get; set; }
        public string RequestingUserName { get; }
        public string UserProfileImage { get; }
        public int PlaySourceId { get; internal set; }
    }
}
