using Google.Apis.YouTube.v3;
using RavelDev.Discord.Bot.Core.Models.YouTube;


namespace RavelDev.Discord.Bot.Core.API
{
    public class YouTubeApi
    {

        public YouTubeApi(YouTubeService ytService) {
            YouTubeService = ytService;
        }

        public YouTubeService YouTubeService { get; private set; }

        public async Task<List<YouTubeMap>> GetPlaylistData(string playlistId)
        {
            var result = new List<YouTubeMap>();
            try
            {
                
                var searchListRequest = YouTubeService.PlaylistItems.List("snippet");
                searchListRequest.PlaylistId = playlistId;
                searchListRequest.MaxResults = 50;


                var searchListResponse = await searchListRequest.ExecuteAsync();
                foreach (var video in searchListResponse.Items)
                {
                    var youtubeUrl = $"https://www.youtube.com/watch?v={video.Snippet.ResourceId.VideoId}";
                    var displayName = video.Snippet.Title;
                    result.Add(new YouTubeMap(SongRequestSource.YouTube, youtubeUrl, displayName));
                }
                var nextPageToken = searchListResponse.NextPageToken;
                while (nextPageToken != null)
                {
                    var nextPageRequest = YouTubeService.PlaylistItems.List("snippet");
                    nextPageRequest.PlaylistId = playlistId;
                    nextPageRequest.MaxResults = 50;
                    nextPageRequest.PageToken = nextPageToken;
                    var nextPageResponse = await nextPageRequest.ExecuteAsync();
                    foreach (var video in nextPageResponse.Items)
                    {
                        var youtubeUrl = $"https://www.youtube.com/watch?v={video.Snippet.ResourceId.VideoId}";
                        var displayName = video.Snippet.Title;
                        result.Add(new YouTubeMap(SongRequestSource.YouTube, youtubeUrl, displayName));
                    }
                    nextPageToken = nextPageResponse.NextPageToken;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while retrieving youtube playlist data. Playist id {playlistId}", ex);
            }


            return result;
        }
    }
}
