using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NekoChattingBot
{
    public class HomeAssistant
    {
        public static WebClient webClient;
        public HomeAssistant()
        {
            webClient = new();
        }
    }
}
