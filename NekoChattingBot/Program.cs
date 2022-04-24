using AsyncAwaitBestPractices;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace NekoChattingBot
{
    internal class Program
    {
        static CommandCache commandCache;

        static async Task Main(string[] args)
        {
            commandCache = new CommandCache();
            commandCache.CacheExisting();

            DateTime lastSeen = DateTime.MinValue;

            var twitchBot = new TwitchBot(TwitchCredentials.botUsername, TwitchCredentials.password);
            twitchBot.Start().SafeFireAndForget();
            //We could .SafeFireAndForget() these two calls if we want to
            for (int i = 0; i < TwitchCredentials.Channels.Length; i++)
                await twitchBot.JoinChannel(TwitchCredentials.Channels[i]);

            if (TwitchCredentials.Channels.Any("nekochattingbot".Contains)) // send the message only if it's in the channel.
                await twitchBot.SendMessage("nekochattingbot", "/me Chatting Bot started");

            twitchBot.OnMessage += async (sender, twitchChatMessage) =>
            {
                try
                {
                    if (twitchChatMessage.Sender.Equals("nekopavel")) lastSeen = DateTime.Now;
                    if (twitchChatMessage.Sender.Equals("thatoneguywhospamspogpega", StringComparison.OrdinalIgnoreCase) && !twitchChatMessage.Message.Contains("Pogpega", StringComparison.OrdinalIgnoreCase))
                        await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me @{twitchChatMessage.Sender} Stare GunL you didn't write \"Pogpega\" in your message, @mods ban him.");
                    Console.WriteLine($"{twitchChatMessage.Sender} said '{twitchChatMessage.Message}'");
                    if (twitchChatMessage.Sender == "streamelements" && twitchChatMessage.Message.Contains("Use code \"BTMC\" for a 30% discount on your order at https://gfuel.ly/2ZplQ3B OkayChamp"))
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "/me gachiHYPER 👆 Use code \"Soque Macaque\" !!!");
                    if (twitchChatMessage.Message.Contains("@nekopavel", StringComparison.OrdinalIgnoreCase))
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

                    string rawMessage = twitchChatMessage.Message;
                    string[] msgArguments = rawMessage.Split(' ');
                    if (msgArguments.Length < 1 || !msgArguments[0].StartsWith("!"))
                        return;
                    string command = msgArguments[0].Substring(1).ToLower();

                    if (!commandCache.Exists(command))
                    {
                        Console.WriteLine($"Command doesn't exist in the cache: {command}.");
                        return;
                    }

                    dynamic commandInstance = commandCache.GetCommandInstance(command);
                    commandInstance.Execute(twitchBot, twitchChatMessage, String.Join(" ", msgArguments.Skip(1).ToArray()));

                    return;

                    /*                                                       */
                    /*   Anything below this isn't executed. Migrate it to   */
                    /*                the new command system :)              */
                    /*                                                       */

                    #region NEEDS_MIGRATION

                    if (twitchChatMessage.Message.Contains("!bored")) await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me HACKERMANS @{twitchChatMessage.Sender} create a chatbot.");


                    

                    else if (twitchChatMessage.Message.StartsWith("!nekorgb", StringComparison.OrdinalIgnoreCase) || twitchChatMessage.Message.StartsWith("Pogpega !nekorgb", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            if (twitchChatMessage.Message.StartsWith("Pogpega"))
                            {
                                twitchChatMessage.Message = twitchChatMessage.Message.Substring(17);
                            }
                            else
                            {
                                twitchChatMessage.Message = twitchChatMessage.Message.Substring(9);
                            }
                            var splitMessage = twitchChatMessage.Message.Trim().Split(' ');
                            //string command = twitchChatMessage.Message.Substring(0, twitchChatMessage.Message.IndexOf(' '));
                            //string device = twitchChatMessage.Message.Substring(command.Length+1, twitchChatMessage.Message.IndexOf(' ',command.Length+1));
                            string command2 = splitMessage[0];
                            string device = splitMessage[1];
                            if (splitMessage.Length == 3)
                            {
                                var colours = splitMessage[2].Split(',');
                                int red = int.Parse(colours[0]);
                                int green = int.Parse(colours[1]);
                                int blue = int.Parse(colours[2]);
                                throw new NotImplementedException($"Chatting This is not done yet. Got command: \"{command2}\", device \"{device}\", r{red},g{green},b{blue}");
                            }
                            else if (splitMessage.Length > 3)
                            {
                                throw new IndexOutOfRangeException();
                            }
                            throw new NotImplementedException($"Chatting This is not done yet. Got command: \"{command2}\", device \"{device}\"");
                            using (var webClient = new System.Net.WebClient())
                            {
                                string webAddr = "http://192.168.1.202:8123/api/services/light/";
                                webClient.Headers.Add("Content-Type", "application/json");
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me Chatting You used the command with the wrong syntax. Correct syntax: !nekorgb on/off/toggle <device> optional:<rgb colour like 250,59,177>");
                        }
                        catch
                        {
                            throw;
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
                            string livestateString = btmcLive.state;
                            bool isLive = bool.Parse(livestateString);
                            string live;
                            if (isLive)
                                live = "YEP";
                            else
                                live = "NOPERS";
                            await twitchBot.SendMessage(twitchChatMessage.Channel, $"/me @{twitchChatMessage.Sender} {live}");
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
                    if (twitchChatMessage.Message.Contains("!kill") && TwitchCredentials.mods.Contains(twitchChatMessage.Sender, StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "SEEYOUNEXTTIME");
                        Environment.Exit(0);
                    }
                    else if (twitchChatMessage.Message.Contains("!kill") && !TwitchCredentials.mods.Contains(twitchChatMessage.Sender, StringComparison.OrdinalIgnoreCase))
                    {
                        await twitchBot.SendMessage(twitchChatMessage.Channel, "OuttaPocket Tssk Only mods and nekopavel can use this command");
                    }
                    //if (twitchChatMessage.Message.Contains("!overlay")&& twitchChatMessage.Channel.Equals("emanfman"))
                    //{
                    //    await twitchBot.SendMessage(twitchChatMessage.Channel, $"@{twitchChatMessage.Sender} TAMPERMONKEY: https://www.tampermonkey.net/ + OVERLAY SCRIPT: https://bit.ly/3iWmdhc (this only overlays pixel art, it doesn't place anything) ");
                    //}
                    #endregion
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
