using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using Eliza;
using System.Configuration;

namespace SkippingRock.SticksBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string user = ConfigurationManager.AppSettings["user"];
            string token = ConfigurationManager.AppSettings["token"];
            string elizaUrl = ConfigurationManager.AppSettings["elizaUrl"];
            ElizaApi eliza = new ElizaApi(elizaUrl, user, token);
            SticksBot bot = new SticksBot(eliza);
            Thread worker = new Thread(bot.DoWork);
            worker.Start();
            while (!worker.IsAlive) ; // loop till he's not there
            Thread.CurrentThread.Join();
        }
    }
}
