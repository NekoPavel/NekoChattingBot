namespace NekoChattingBot.Commands
{
    public class Test : Command
    {
        public static string Name = "test";

        public override async void Execute(TwitchBot botInstance, string rawArgs)
        {
            await botInstance.SendMessage("pancakes", "Test!");
        }
    }
}