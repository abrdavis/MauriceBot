using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Entity.Models.YouTube
{
    public class YouTubePlay
    {
        public int YouTubePlayId { get; set; }
        public required string VideoTitle { get; set; }
        public required string YouTubeVideoId { get; set; }
        public DateTime PlayDate { get; set; }
        [Column(TypeName = "bigint")]
        public ulong DiscordUserId { get; set; }

        [Column(TypeName = "bigint")]
        public long GuildId { get; set; }
        public int YouTubePlaySourceId { get; set; }
        public required string SourceUrl { get; set; }
    }
}
