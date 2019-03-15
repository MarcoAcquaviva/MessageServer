using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerMessagingApp
{
    public class GameServer
    {
        private Dictionary<EndPoint, GameClient> clientsTable;
        private IGameTransport transport;
        private IMonotonicClock clock;
        private float currentNow;

        public GameServer(IGameTransport gameTransport, IMonotonicClock clock)
        {
            this.transport = gameTransport;
            this.clock = clock;
            clientsTable = new Dictionary<EndPoint, GameClient>();
        }

        public void Run()
        {
            Console.WriteLine("server started");
            while (true)
            {
                SingleStep();
            }
        }

        public void SingleStep()
        {
            EndPoint sender = transport.CreateEndPoint();
            byte[] data = transport.Recv(256, ref sender);
            if (data != null && data.Length > 0)
            {
                Console.WriteLine("Recive message from {0} with {1} ", sender, data[0]);
                string text = Encoding.UTF8.GetString(data);
                Packet packet = new Packet(Reverse(text));
                SendToClient(packet, sender);
            }

            foreach (GameClient client in clientsTable.Values)
            {
                client.Process();
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public void SendToClient(Packet packet, EndPoint sender)
        {
            transport.Send(packet.GetData(), sender);
            Console.WriteLine("Sendig Data to {0} with {1}", sender, packet.GetData()[0]);
        }

        public bool Send(Packet packet, EndPoint endPoint)
        {
            return transport.Send(packet.GetData(), endPoint);
        }

        public float Now
        {
            get
            {
                return currentNow;
            }
        }

        public int NumClients
        {
            get { return clientsTable.Count; }
        }
    }
}
