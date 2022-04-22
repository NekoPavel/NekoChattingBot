using static NekoChattingBot.TwitchBot;

namespace NekoChattingBot
{
    /// <summary>
    /// Twitch bot command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The name of the command (lowercase, without prefix), that will be used to execute it. Example: `skin`.
        /// </summary>
        public string ExecutionName;
        /// <summary>
        /// Method that will be executed, once command is fired. Should be async if any interaction with <see cref="TwitchBot"/> is planned.
        /// </summary>
        /// <param name="rawArgs">Raw unsplit string of arguments provided after the command.</param>
        public abstract void Execute(TwitchBot botInstance, TwitchChatMessage twitchChatMessage, string rawArgs);
    }
}
