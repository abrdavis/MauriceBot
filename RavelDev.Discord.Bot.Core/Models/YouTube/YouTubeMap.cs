using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Models.YouTube
{
    public class YouTubeMap
    {
        public YouTubeMap()
        {
            MapSource = SongRequestSource.Spotify;
            YouTubeUrl = string.Empty;
            AlbumUrl = string.Empty;
        }
        public YouTubeMap(int mapSource, string youTubeUrl, string displayName, string albumUrl = "")
        {
            MapSource = mapSource;
            YouTubeUrl = youTubeUrl;
            DisplayName = displayName;
            AlbumUrl = albumUrl;
        }

        public string AlbumUrl { get; set; }
        public string YouTubeUrl { get; set; }
        public int MapSource { get; set; }
        public string? DisplayName { get; set; }
    }

    public class SongRequestSource
    {
        public static int YouTube = 1;

        public static int Spotify = 2;
    }
}
