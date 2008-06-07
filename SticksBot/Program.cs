using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace SkippingRock.SticksBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = "vS3JHbsnKbGybTFwFjHVo5tzh";
            string user = "aiSticks";
            SticksBot bot = new SticksBot(user,token);
            Thread worker = new Thread(bot.DoWork);
            worker.Start();
            while (!worker.IsAlive) ; // loop till he's there
            Thread.CurrentThread.Join();
        }
    }
}
