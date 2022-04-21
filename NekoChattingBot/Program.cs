using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using AsyncAwaitBestPractices;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace NekoChattingBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DateTime lastSeen = DateTime.MinValue;
            string mods = "ThatOneGuyWhoSpamsPogpega, aeonim, andros18, bennyoberwinch, bigtimemassivecash, binfy, bluecrystal004, boxbox, chase55t, chrchie, cloud9, deadrote, derpyfoxplayz, digitalhypno, dios_dong, doremy, elcheer, eleeement_, emanfman, enyoti, fossabot, happystick, honmi, intlcoco, itswinter, jamiethebull, joiechii, kaoran, kerneon, ksn24, l3lackshark, littleendu, luckfire, mikuia, mrchompysaur, mrdutchboi, mrnolife_, nightbot, olibomby, oralekin, piscator_, rainbowmeeps, ryotou, shigetora, shugi, slpchatbot, soarnathan, soran2202, streamelements, stunterletsplay, thepoon, theramu, therealzachtv, toekneered, toybickler, tru3o_o, tuonto, warpworldbot, wholewheatpete, yazzehh, ypaperr, zarrah, zonelouise, nekopavel";
            string password = Environment.GetEnvironmentVariable("TWITCH_OAUTH", EnvironmentVariableTarget.User);
            string botUsername = "NekoChattingBot";

            var twitchBot = new TwitchBot(botUsername, password);
            twitchBot.Start().SafeFireAndForget();
            //We could .SafeFireAndForget() these two calls if we want to
            await twitchBot.JoinChannel("btmc");
            await twitchBot.JoinChannel("nekopavel");
            await twitchBot.JoinChannel("nekochattingbot");
            //await twitchBot.JoinChannel("emanfman");
            await twitchBot.JoinChannel("thatoneguywhospamspogpega");
            await twitchBot.SendMessage("nekochattingbot", "/me Chatting Bot started");

            twitchBot.OnMessage += async (sender, twitchChatMessage) =>
            {
                try
                {
                    if (twitchChatMessage.Sender.Equals("nekopavel")) lastSeen = DateTime.Now;
                    if (twitchChatMessage.Sender.Equals("thatoneguywhospamspogpega", StringComparison.OrdinalIgnoreCase) && !twitchChatMessage.Message.Contains("Pogpega", StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me @{twitchChatMessage.Sender} Stare GunL you didn't write \"Pogpega\" in your message, @mods ban him.");
                    }
                    Console.WriteLine($"{twitchChatMessage.Sender} said '{twitchChatMessage.Message}'");
                    if (twitchChatMessage.Sender == "streamelements" && twitchChatMessage.Message.Contains("Use code \"BTMC\" for a 30% discount on your order at https://gfuel.ly/2ZplQ3B OkayChamp"))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "/me gachiHYPER 👆 Use code \"Soque Macaque\" !!!");
                    }
                    else if (twitchChatMessage.Message.Contains("!bored")) await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me HACKERMANS @{twitchChatMessage.Sender} create a chatbot.");
                    #region disabled
                    //else if (twitchChatMessage.Message.Contains("!red"))
                    //{
                    //    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.1.202:8123/api/webhook/twitch-bot");
                    //    httpWebRequest.Method = "POST";
                    //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    //    {
                    //        streamWriter.Write("");
                    //    }

                    //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //    {
                    //        var result = streamReader.ReadToEnd();
                    //    }
                    //}
                    #endregion
                    else if (twitchChatMessage.Message.Contains("!nekonp"))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                            var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/media_player.spotify_pavel_kuzminov");
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
                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Currently listening to: {songName} by {songArtist} from the album {songAlbum}. Song link: {songLink}");
                            }
                            else
                            {
                                await twitchBot.SendMessage(twitchChatMessage.Channel, "No music is playing right now");
                            }
                        }
                    }
                    else if (twitchChatMessage.Message.Contains("!rice"))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                            var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/switch.xiaomi_miio_cooker");
                            // Now parse with JSON.Net
                            dynamic riceCooker = JObject.Parse(json);

                            json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_mode");
                            // Now parse with JSON.Net
                            dynamic riceCookerState = JObject.Parse(json);

                            string state = riceCookerState.state;

                            if (riceCooker.state == "on" || state == "AutoKeepWarm" || state == "PreCook")
                            {

                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_menu");
                                dynamic riceCookerMode = JObject.Parse(json);

                                int mode = riceCookerMode.state;

                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_remaining");
                                dynamic riceCookerRemaining = JObject.Parse(json);

                                string minutes = riceCookerRemaining.state;

                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_duration");
                                dynamic riceCookerDuration = JObject.Parse(json);

                                string duration = riceCookerDuration.state;

                                //sensor.xiaomi_miio_cooker_temperature
                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_temperature");
                                dynamic riceCookerTemperature = JObject.Parse(json);

                                string temperature = riceCookerTemperature.state;
                                //sensor.xiaomi_miio_cooker_stage_description
                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_stage_description");
                                dynamic riceCookerStageDescription = JObject.Parse(json);

                                string stageDescription = riceCookerStageDescription.state;
                                //sensor.xiaomi_miio_cooker_stage_name
                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_stage_name");
                                dynamic riceCookerStageName = JObject.Parse(json);

                                string stage = riceCookerStageName.state;




                                switch (mode)
                                {
                                    case 1:
                                        switch (state)
                                        {
                                            case "Running":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Rice is cooking, {minutes} minutes remaining until it is done. Current temperature: {temperature}°C Current stage is: {stage}, {stageDescription}.");
                                                break;
                                            case "Waiting":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, "Ricecooker is idle.");
                                                break;
                                            case "AutoKeepWarm":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"The rice is done, it has been kept warm for {minutes} minutes now.");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    case 4:
                                        switch (state)
                                        {
                                            case "Running":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Rice is being reheated, it has been kept warm for {minutes} minutes now. Current temperature is {temperature}°C");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    case 3:
                                        switch (state)
                                        {
                                            case "Running":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Making rice congee (rice cum), {minutes} minutes remaining until it is done. Current stage is: {stage}, {stageDescription}.");
                                                break;
                                            case "AutoKeepWarm":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"The rice congee is done (rice cum), it has been kept warm for {minutes} minutes now.");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    case 258:
                                        switch (state)
                                        {
                                            case "Running":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Rice is being reheated, it will be done in {minutes} minutes. Current temperature is {temperature}°C. Current stage is: {stage}, {stageDescription}.");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    case 260:
                                        switch (state)
                                        {
                                            case "Running":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Making tasty rice, it will be done in {minutes} minutes. Current temperature is {temperature}°C. Current stage is: {stage}, {stageDescription}.");
                                                break;
                                            case "PreCook":
                                                string timeOutput = "";
                                                TimeSpan startIn = TimeSpan.FromMinutes(double.Parse(minutes));
                                                //$"modCheck He seems to be in chat. (Last chatted {lastSeenAgo.TotalMinutes} minutes ago)"
                                                if (startIn.Hours > 0)
                                                    timeOutput = $"({startIn.Hours} hour(s), {startIn.Minutes} minute(s) and {startIn.Seconds} second(s))";
                                                else if (startIn.Minutes > 0)
                                                    timeOutput = $"({startIn.Minutes} minute(s) and {startIn.Seconds} second(s))";
                                                else if (startIn.Seconds > 0)
                                                    timeOutput = $"({startIn.Seconds} second(s))";
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Rice cooker is scheduled to start cooking tasty rice in {timeOutput}.");
                                                break;
                                            case "AutoKeepWarm":
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"The tasty rice is done, it has been kept warm for {minutes} minutes now.");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    default:
                                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"Chatting Unknown mode: {mode}");
                                        break;
                                }



                            }
                            else
                            {
                                DateTime dateTime = DateTime.Now;
                                string lastRice = riceCooker.last_updated;


                                TimeSpan timeDiff = dateTime - DateTime.Parse(lastRice, CultureInfo.CreateSpecificCulture("se-SV"), DateTimeStyles.AllowInnerWhite);

                                string timeOutput = "";
                                if (timeDiff.Days > 0)
                                    timeOutput = timeDiff.Days.ToString() + " days, " + timeDiff.Hours.ToString() + " hours and " + timeDiff.Minutes.ToString() + " minutes";
                                else if (timeDiff.Hours > 0)
                                    timeOutput = timeDiff.Hours.ToString() + " hours and " + timeDiff.Minutes.ToString() + " minutes";
                                else if (timeDiff.Minutes > 0)
                                    timeOutput = timeDiff.Minutes.ToString() + " minutes";

                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"The rice cooker is off. It was last on {timeOutput} ago");
                            }
                        }
                    }
                    else if (twitchChatMessage.Message.Contains("@nekopavel", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                            var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/input_boolean.pavel_sleep_sleeping");
                            // Now parse with JSON.Net
                            dynamic sleep = JObject.Parse(json);
                            string sleeping = sleep.state;
                            /*if (sleeping.Equals("on", StringComparison.OrdinalIgnoreCase) && twitchChatMessage.Message.Contains("HOLYSHITSTHISISIMPORTANTWAKEUP!", StringComparison.OrdinalIgnoreCase))
                            {
                                string webAddr = "http://192.168.1.202:8123/api/services/notify/mobile_app_oneplus_7_pro";
                                webClient.Headers.Add("Content-Type", "application/json");


                                string jsonRequest = JsonSerializer.Serialize(new
                                {
                                    title = $"Important pinged by {twitchChatMessage.Sender} in {twitchChatMessage.Channel}",
                                    message = twitchChatMessage.Message
                                });

                                var result = webClient.UploadString(webAddr, "POST", jsonRequest);
                                await twitchBot.SendMessage(twitchChatMessage.Channel, "DinkDonk He has been pinged on his phone now and the room light is now on.");
                            }
                            else*/
                            if (sleeping.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {/* If you actually want him to get a notification include this in your message \"HOLYSHITSTHISISIMPORTANTWAKEUP!\". (Note: this will flash his lights and most likely actually wake him up, please don't abuse.*/
                                await twitchBot.SendMessage(twitchChatMessage.Channel, "Bedge He is asleep.");
                            }
                            else
                            {
                                TimeSpan lastSeenAgo = DateTime.Now - lastSeen;
                                if (TimeSpan.FromMinutes(1.5) < lastSeenAgo && lastSeenAgo < TimeSpan.FromMinutes(5))
                                {
                                    string timeOutput = "";
                                    //$"modCheck He seems to be in chat. (Last chatted {lastSeenAgo.TotalMinutes} minutes ago)"
                                    if (lastSeenAgo.Minutes > 0)
                                        timeOutput = $"(Last chatted {lastSeenAgo.Minutes} minute(s) and {lastSeenAgo.Seconds} second(s) ago)";
                                    else if (lastSeenAgo.Seconds > 0)
                                        timeOutput = $"(Last chatted {lastSeenAgo.Seconds} second(s) ago)";
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "modCheck He seems to be in chat. " + timeOutput);
                                }
                                else if (lastSeenAgo > TimeSpan.FromMinutes(4))
                                {
                                    string webAddr = "http://192.168.1.202:8123/api/services/notify/mobile_app_oneplus_7_pro";
                                    webClient.Headers.Add("Content-Type", "application/json");


                                    string jsonRequest = JsonSerializer.Serialize(new
                                    {
                                        title = $"Pinged by {twitchChatMessage.Sender} in {twitchChatMessage.Channel}",
                                        message = twitchChatMessage.Message
                                    });

                                    var result = webClient.UploadString(webAddr, "POST", jsonRequest);


                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "BOGGED He has been pinged on his phone now.");

                                }

                            }

                        }
                    }
                    else if (twitchChatMessage.Message.Contains("!nekorgb", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new NotImplementedException("Chatting This is not done yet.");
                        using (var webClient = new System.Net.WebClient())
                        {

                            string webAddr = "http://192.168.1.202:8123/api/services/notify/mobile_app_oneplus_7_pro";
                            webClient.Headers.Add("Content-Type", "application/json");
                        }
                    }
                    else if (twitchChatMessage.Message.Contains("!math 9+10") || twitchChatMessage.Message.Contains("!math 10+9"))
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me Chatting @{twitchChatMessage.Sender} 21");
                    else if (twitchChatMessage.Message.Contains("!math 77+33") || twitchChatMessage.Message.Contains("!math 33+77"))
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me Chatting @{twitchChatMessage.Sender} 100");
                    else if (twitchChatMessage.Message.Contains("Boolin") && twitchChatMessage.Message.Contains("?") && twitchChatMessage.Sender.Equals("mrdutchboi", StringComparison.OrdinalIgnoreCase))
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"Zxjq we boolin Boolin");
                    else if (twitchChatMessage.Message.Contains("!isedlive"))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                            var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.btmc");
                            // Now parse with JSON.Net
                            dynamic btmcLive = JObject.Parse(json);
                            string liveState = btmcLive.state;
                            await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me {liveState}");
                        }
                    }
                    else if (twitchChatMessage.Message.Contains("modCheck", StringComparison.OrdinalIgnoreCase) && (twitchChatMessage.Message.Contains("nekopavel", StringComparison.OrdinalIgnoreCase) || twitchChatMessage.Message.Contains("@nekopavel", StringComparison.OrdinalIgnoreCase)))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                            var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/person.pavel");
                            // Now parse with JSON.Net
                            dynamic pavel = JObject.Parse(json);
                            string location = pavel.state;
                            switch (location)
                            {
                                case "home":
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me HACKERMANS He seems to be at home.");
                                    break;
                                case "store":
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me PepegaCredit He's buying groceries.");
                                    break;
                                case "store2":
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me PepegaCredit He's buying groceries.");
                                    break;
                                case "gym":
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me GIGACHAD He's at the gym.");
                                    break;
                                case "work":
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me Tasty hackingCD He's at the office.");
                                    break;
                                default:
                                    await twitchBot.SendMessage(twitchChatMessage.Channel, "/me 4Shrug He's not in one of the known locations.");
                                    break;
                            }
                        }
                    }
                    if (twitchChatMessage.Message.Contains("!kill") && mods.Contains(twitchChatMessage.Sender, StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "SEEYOUNEXTTIME");
                        Environment.Exit(0);
                    }
                    else if (twitchChatMessage.Message.Contains("!kill") && !mods.Contains(twitchChatMessage.Sender, StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "OuttaPocket Tssk Only mods and nekopavel can use this command");
                    }
                    //if (twitchChatMessage.Message.Contains("!overlay")&& twitchChatMessage.Channel.Equals("emanfman"))
                    //{
                    //    await twitchBot.SendMessage(twitchChatMessage.Channel, $"@{twitchChatMessage.Sender} TAMPERMONKEY: https://www.tampermonkey.net/ + OVERLAY SCRIPT: https://bit.ly/3iWmdhc (this only overlays pixel art, it doesn't place anything) ");
                    //}
                }
                catch (Exception e)
                {
                    await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me Chatting L + Ratio + Rip Bozo + {e.Message}");
                }
            };





            await Task.Delay(-1);


        }
    }
}
