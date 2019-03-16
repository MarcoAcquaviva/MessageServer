using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMessagingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GameClient client = new GameClient("192.168.1.197", 9999);
            client.Run();
            
        }
    }
}
