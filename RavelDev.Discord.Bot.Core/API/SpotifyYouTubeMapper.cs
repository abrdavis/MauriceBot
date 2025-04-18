using Google.Apis.YouTube.v3;
using RavelDev.Discord.Bot.Core.Models.YouTube;
using RavelDev.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.API
{
    public class SpotifyYouTubeMapper
    {

        public SpotifyYouTubeMapper(YouTubeService youTubeService,
            SpotifyWebApi spotifyWebApi)
        {
            YouTubeService = youTubeService;
            SpotifyWebApi = spotifyWebApi;
        }

        public YouTubeService YouTubeService { get; }
        public SpotifyWebApi SpotifyWebApi { get; }

        public async Task<YouTubeMap?> GetYouTubeDataForSpotifyTrack(string spotifyTrackId)
        {
            if (!SpotifyWebApi.IsConnected)
            {
                await SpotifyWebApi.ConnectAndSetAccessToken();
            }

            YouTubeMap? result = null;
            var spotifyTrackInformation = await SpotifyWebApi.GetTrackInformation(trackId: spotifyTrackId);

            if (spotifyTrackInformation == null)
            {
                return result;
            }

            if (spotifyTrackInformation.artists == null || !spotifyTrackInformation.artists.Any())
            {
                return result;
            }

            var searchListRequest = YouTubeService.Search.List("snippet");
            var spotifyTrackDisplay = $"{spotifyTrackInformation.artists.FirstOrDefault()?.name} - {spotifyTrackInformation.name}";
            searchListRequest.Q = spotifyTrackDisplay;
            searchListRequest.MaxResults = 50;
            searchListRequest.Type = "video";

            var albumUrl = spotifyTrackInformation.album.images.FirstOrDefault()?.url;
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var youTubeId = searchListResponse.Items[0].Id.VideoId;
            return new YouTubeMap(mapSource: SongRequestSource.Spotify,
                                  youTubeUrl: $"https://www.youtube.com/watch?v={youTubeId}",
                                  displayName: spotifyTrackDisplay,
                                  albumUrl: albumUrl ?? string.Empty);
        }
    }
}
