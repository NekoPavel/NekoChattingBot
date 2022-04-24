using Newtonsoft.Json.Linq;
using System;
using System.Net;
using static NekoChattingBot.TwitchBot;

namespace NekoChattingBot.Commands
{
    public class SpotifyNowPlaying : Command
    {
        public static string Name = "nekonp";

        public override async void Execute(TwitchBot botInstance, TwitchChatMessage twitchChatMessage, string rawArgs)
        {
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                Uri uri = new Uri("http://192.168.1.202:8123/api/states/media_player.spotify_pavel_kuzminov");
                var json = await webClient.DownloadStringTaskAsync(uri);


                // Now parse with JSON.Net
                dynamic spotify = JObject.Parse(json);
                string isPlaying = spotify.state;

                if (isPlaying == "playing")
                {
                    string songName = spotify.attributes.media_title;
                    string songArtist = spotify.attributes.media_artist;
                    string songAlbum = spotify.attributes.media_album_name;
                    string songId = spotify.attributes.media_content_id;
                    string songLink = "https://open.spotify.com/track/" + songId.Substring(14); //spotify:track:1Bv2pFGVUQmGQZcpLgvn7K
                    await botInstance.SendMessage(twitchChatMessage.Channel, $"Currently listening to: {songName} by {songArtist} from the album {songAlbum}. Song link: {songLink}");
                }
                else
                {
                    await botInstance.SendMessage(twitchChatMessage.Channel, "No music is playing right now");
                }
            }
        }
    }
}
