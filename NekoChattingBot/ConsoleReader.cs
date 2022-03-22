using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekoChattingBot
{
    internal class ConsoleReader
    {

        public ConsoleReader()
        {

        }

        private async Task<string> GetInputAsync()
        {
            return (Task.Run(() => Console.ReadLine())).Result;
        }

    }
}
