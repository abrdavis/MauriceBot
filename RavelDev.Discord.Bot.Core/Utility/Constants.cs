using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Utility
{
    //^.*(youtu.be\/|list=)([^#\&\?]*)(&v=[a-z_A-Z0-9\\-]{11})|(.*)
    internal class RegExPatterns
    {
        public static string YouTubeVideoUrl = "(http(s|):|)\\/\\/(www\\.|)yout(.*?)\\/(embed\\/|watch.*?v=|)([a-z_A-Z0-9\\-]{11})";
        public static string YouTubePlaylistUrl = "https?:\\/\\/www\\.youtube\\.com\\/.*\\?.*\\blist=([A-Za-z0-9_-]{16,32})";
        public static string SpotifyTrackUrl = "\\bhttps?:\\/\\/[^/]*\\bspotify\\.com\\/track\\/([^\\s?]+)";
        public static string LastNonWordCharacter = "([\\W]*$)";
    }


}
