using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Commands.YouTube
{
    public class YouTubeStreamInstance
    {
        private CancellationTokenSource _cancellationTokenSource;

        public YouTubeStreamInstance(CancellationTokenSource cancellationToken,
            SocketVoiceChannel voiceChannel)
        {
            _cancellationTokenSource = cancellationToken;
            VoiceChannel = voiceChannel;
        }


        private SocketVoiceChannel VoiceChannel { get; set; }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public async Task LeaveChannel()
        {
            await VoiceChannel.DisconnectAsync();
        }
    }
}
