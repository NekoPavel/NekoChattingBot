using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using static NekoChattingBot.TwitchBot;

namespace NekoChattingBot.Commands
{
    public class GetRicecookerStatus : Command
    {
        public static string Name = "nekonp";

        public override async void Execute(TwitchBot botInstance, TwitchChatMessage twitchChatMessage, string rawArgs)
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
    }
}