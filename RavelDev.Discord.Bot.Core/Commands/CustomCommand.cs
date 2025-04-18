using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Commands
{
    public abstract class CustomCommand
    {
        public required string CommandPrefix { get; set; }
        public required int MaxDiscordMessageLength { get; set; }
        public ulong ClientId { get; internal set; }



       public virtual void Init()
        {
            return;
        }
        public abstract bool IsCommand(string messagePrefix);
        public abstract Task ExecuteAsync(SocketUserMessage msg, SocketGuildUser user);


    }
}
