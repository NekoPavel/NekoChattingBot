using static NekoChattingBot.TwitchBot;

namespace NekoChattingBot.Commands
{
    public class Test : Command
    {
        public static string Name = "test";

        public override async void Execute(TwitchBot botInstance, TwitchChatMessage twitchChatMessage, string rawArgs)
        {
            await botInstance.SendMessage(twitchChatMessage.Channel, "Test!");
        }
    }
}