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
            DateTime lastSeen = DateTime.Now;

            string password = Environment.GetEnvironmentVariable("TWITCH_OAUTH", EnvironmentVariableTarget.User);
            string botUsername = "NekoChattingBot";

            var twitchBot = new TwitchBot(botUsername, password);
            twitchBot.Start().SafeFireAndForget();
            //We could .SafeFireAndForget() these two calls if we want to
            await twitchBot.JoinChannel("btmc");
            await twitchBot.JoinChannel("nekopavel");
            await twitchBot.SendMessage("nekopavel", "/me Chatting Bot started");

            twitchBot.OnMessage += async (sender, twitchChatMessage) =>
            {
                try
                {
                    Console.WriteLine($"{twitchChatMessage.Sender} said '{twitchChatMessage.Message}'");
                    //Listen for !hey command
                    //if (twitchChatMessage.Message.StartsWith("!hey"))
                    //{
                    //    await twitchBot.SendMessage(twitchChatMessage.Channel, $"Hey there {twitchChatMessage.Sender} peepoHey");
                    //}
                    if (twitchChatMessage.Sender == "streamelements" && twitchChatMessage.Message.Contains("Use code \"BTMC\" for a 10% discount on your order at https://gfuel.ly/2ZplQ3B OkayChamp"))
                    {
                        Console.WriteLine("Code message detected!");
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "/me PogO look at all these bots Stare");
                    }
                    else if (twitchChatMessage.Message.StartsWith("!bored")) await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me HACKERMANS @{twitchChatMessage.Sender} create a chatbot.");
                    #region disabled
                    //else if (twitchChatMessage.Message.Contains("!test"))
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

                            if (riceCooker.state == "on" || state == "AutoKeepWarm")
                            {

                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_menu");
                                dynamic riceCookerMode = JObject.Parse(json);

                                int mode = riceCookerMode.state;

                                json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.xiaomi_miio_cooker_remaining");
                                dynamic riceCookerRemaining = JObject.Parse(json);

                                string minutes = riceCookerRemaining.state;

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
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Rice is being reheated, it has been kept warm for {minutes} minutes now.");
                                                break;
                                            default:
                                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"Unknown state: {state}");
                                                break;
                                        }
                                        break;
                                    default:
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
                                else if(timeDiff.Minutes > 0)
                                    timeOutput = timeDiff.Minutes.ToString() + " minutes";

                                await twitchBot.SendMessage(twitchChatMessage.Channel, $"The rice cooker is off. It was last on {timeOutput} ago");
                            }
                        }
                    }
                    else if (twitchChatMessage.Message.Contains("@nekopavel", StringComparison.OrdinalIgnoreCase))
                    {

                        if (lastSeen - DateTime.Now < TimeSpan.FromMinutes(5))
                        {
                            await twitchBot.SendMessage(twitchChatMessage.Channel, "modCheck He seems to be in chat");
                        }
                        else
                        {
                            string webAddr = "http://192.168.1.202:8123/api/services/notify/mobile_app_oneplus_7_pro";

                            using (var client = new System.Net.WebClient())
                            {

                                client.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                                client.Headers.Add("Content-Type", "application/json");


                                string json = JsonSerializer.Serialize(new
                                {
                                    title = $"Pinged by {twitchChatMessage.Sender} in {twitchChatMessage.Channel}",
                                    message = twitchChatMessage.Message
                                });

                                var result = client.UploadString(webAddr, "POST", json);


                                await twitchBot.SendMessage(twitchChatMessage.Channel, "@NekoPavel has been pinged on his phone");
                            }
                        }

                    }
                    else if (twitchChatMessage.Sender.Equals("thatoneguywhospamspogpega") && !twitchChatMessage.Message.Contains("Pogpega", StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me @{twitchChatMessage.Sender} Stare you didn't write \"Pogpega\" in your message, @mods ban him.");
                    }
                    else if (twitchChatMessage.Sender.Equals("nekopavel"))  lastSeen = DateTime.Now; 
                }
                catch (Exception e)
                {
                    await twitchBot.SendMessage(twitchChatMessage.Channel, e.Message);
                }
            };





            await Task.Delay(-1);


        }
    }
}
