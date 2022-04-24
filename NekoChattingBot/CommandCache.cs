using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NekoChattingBot
{
    /// <summary>
    /// Cache manager for registered commands.
    /// </summary>
    public class CommandCache
    {
        /// <summary>
        /// Cache storage for registered commands. Use <see cref="GetCommandInstance(string)"/> to get command's instance.
        /// </summary>
        IDictionary<string, Type> Cache;
        public CommandCache()
        {
            Cache = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Caches already registered commands into <see cref="Cache"/>
        /// </summary>
        public void CacheExisting()
        {
            var commands = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.IsClass && t.Namespace == "NekoChattingBot.Commands")
                            .ToList();

            Regex reflectionOnlyClasses = new Regex(@"<(\w+)?>(\w+)?");

            foreach (var _command in commands)
            {
                if (reflectionOnlyClasses.IsMatch(_command.Name))
                    continue;
                Cache.Add($"{_command.GetField("Name").GetValue(null)}", _command);
            }
            Console.WriteLine(String.Format("Cached {0} command{1}.", Cache.Count, Cache.Count == 1 ? "" : "s"));
        }

        public bool Exists(string _commandName) => Cache.ContainsKey(_commandName);

        /// <summary>
        /// Get an instance of already existing and cached command.
        /// </summary>
        /// <param name="commandName">Command string name (must be lowercase)</param>
        /// <returns><see cref="Command"/></returns>
        public object GetCommandInstance(string commandName) => Activator.CreateInstance(Cache[commandName]);
    }
}
