﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekoChattingBot
{
    internal class ConsoleReader
    {
        private async Task<string> GetInputAsync() => (Task.Run(() => Console.ReadLine())).Result;

    }
}
