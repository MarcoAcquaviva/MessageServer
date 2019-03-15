using System;
using System.Net;
using System.Collections.Generic;

namespace ServerMessagingApp
{
    class GameClient
    {
        private EndPoint endPoint;
        private GameServer server;
        private Queue<Packet> sendQueue;
        private string message;

        public GameClient(EndPoint endPoint, GameServer server)
        {
            this.endPoint = endPoint;
            this.server = server;
            sendQueue = new Queue<Packet>();
        }

        public void UserInput()
        {
            Console.WriteLine("\n@ Insert a message:");
            message = Console.ReadLine();
            Packet packet = new Packet(0,message);
            sendQueue.Enqueue(packet);
        }

        public void Process()
        {
            int packetsInQueue = sendQueue.Count;
            for (int i = 0; i < packetsInQueue; i++)
            {
                Packet packet = sendQueue.Dequeue();
                if (server.Send(packet, endPoint))
                {
                    Console.WriteLine("[+] packet {0} successfully sent to server", packet.Id);
                }
                else
                {
                    Console.WriteLine("/!\\ failed to send packet {0}", packet.Id);
                }
            }
        }
    }
}
