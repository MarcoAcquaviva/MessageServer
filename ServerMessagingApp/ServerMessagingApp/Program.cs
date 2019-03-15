using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMessagingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GameTransportIPv4 transport = new GameTransportIPv4();
            transport.Bind("192.168.3.82", 9999);
            GameServer server = new GameServer(transport, null);
            server.Run();
        }
    }
}
