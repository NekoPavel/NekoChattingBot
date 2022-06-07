using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NekoChattingBot
{
    public class TwitchBot
    {

        const string ip = "irc.chat.twitch.tv";
        const int port = 6697;

        private string nick;
        private string password;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private TaskCompletionSource<int> connected = new TaskCompletionSource<int>();

        private bool isBtmcLive;

        public event TwitchChatEventHandler OnMessage = delegate { };
        public delegate void TwitchChatEventHandler(object sender, TwitchChatMessage e);

        public class TwitchChatMessage : EventArgs
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public string Channel { get; set; }
        }

        public TwitchBot(string nick, string password)
        {
            this.nick = nick;
            this.password = password;
        }

        private string lastmessage = ""; //󠀀

        public async Task SendMessage(string channel, string message)
        {
            
            if (channel.Equals("btmc") && (message!= "SEEYOUNEXTTIME" || message!=("Yeah we boolin Boolin")))
            {
                //http://192.168.1.202:8123/api/states/sensor.btmc
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.Authorization, Environment.GetEnvironmentVariable("HA_KEY", EnvironmentVariableTarget.User));
                    var json = webClient.DownloadString("http://192.168.1.202:8123/api/states/sensor.btmc");
                    // Now parse with JSON.Net
                    dynamic btmcLive = JObject.Parse(json);
                    string liveState = btmcLive.state;
                    if (!liveState.Equals("false", StringComparison.OrdinalIgnoreCase) && !isBtmcLive)
                    {
                        channel = "nekochattingbot";
                    }
                }
            }
            if (message == lastmessage)
                message += " 󠀀";
            await connected.Task;
            await streamWriter.WriteLineAsync($"PRIVMSG #{channel} :{message}");
            lastmessage = message;
        }

        public async Task JoinChannel(string channel)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"JOIN #{channel}");
        }
        
        public async Task LeaveChannel(string channel)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"PART #{channel}");
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        public async Task Start()
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            SslStream sslStream = new SslStream(
                tcpClient.GetStream(),
                false,
                ValidateServerCertificate,
                null
            );
            await sslStream.AuthenticateAsClientAsync(ip);
            streamReader = new StreamReader(sslStream);
            streamWriter = new StreamWriter(sslStream) { NewLine = "\r\n", AutoFlush = true };

            await streamWriter.WriteLineAsync($"PASS {password}");
            await streamWriter.WriteLineAsync($"NICK {nick}");

            connected.SetResult(0);
            while (true)
            {
                string line = await streamReader.ReadLineAsync();
                // Console.WriteLine(line);

                string[] split = line.Split(" ");
                //PING :tmi.twitch.tv
                //Respond with PONG :tmi.twitch.tv
                if (line.StartsWith("PING"))
                {
                    Console.WriteLine($"PING {split[1]}");
                    await streamWriter.WriteLineAsync($"PONG {split[1]}");
                }
                //else if (line.Contains("Chatting"))
                //{
                //    Console.WriteLine("Chatting");
                //    await streamWriter.WriteLineAsync($"PRIVMSG #btmc :Chatting");
                //}
                //else if (line.Contains(":streamelements!streamelements@streamelements.tmi.twitch.tv PRIVMSG #btmc :Use code \"BTMC\" for a 10% discount on your order at https://gfuel.ly/2ZplQ3B OkayChamp"))
                //{
                //    await streamWriter.WriteLineAsync($"PRIVMSG #btmc :Chatting");
                //}
                if (split.Length > 2 && split[1] == "PRIVMSG")
                {
                    //:mytwitchchannel!mytwitchchannel@mytwitchchannel.tmi.twitch.tv 
                    // ^^^^^^^^
                    //Grab this name here
                    int exclamationPointPosition = split[0].IndexOf("!");
                    string username = split[0].Substring(1, exclamationPointPosition - 1);
                    //Skip the first character, the first colon, then find the next colon
                    int secondColonPosition = line.IndexOf(':', 1);//the 1 here is what skips the first character
                    string message = line.Substring(secondColonPosition + 1);//Everything past the second colon
                    string channel = split[2].TrimStart('#');

                    OnMessage(this, new TwitchChatMessage
                    {
                        Message = message,
                        Sender = username,
                        Channel = channel
                    });
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
            
        }
        public async Task Stop()
        {

        }

    }
}
