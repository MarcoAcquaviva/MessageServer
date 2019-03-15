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
        private delegate void GameCommand(byte[] data, EndPoint sender);
        private Dictionary<byte, GameCommand> commandsTable;

        private Dictionary<EndPoint, GameClient> clientsTable;

        private IGameTransport transport;
        private IMonotonicClock clock;
        private float currentNow;

        #region Server Command Methods

        public void ReverseString(byte[] data, EndPoint sender)
        {
            if (data != null && data.Length > 1)
            {
                Console.WriteLine("Recive message from {0} with {1} ", sender, data[0]);
                string text = Encoding.UTF8.GetString(data);
                RemoveIndexCharFromString(ref text, 0);
                Packet packet = new Packet(0, Reverse(text));
                SendToClient(packet, sender);
            }
        }

        public void Add(byte[] data, EndPoint sender)
        {
            if (data != null && data.Length == 10)
            {
                byte commandID = data[0];
                Packet packet = new Packet();

                char type = Encoding.UTF8.GetChars(data, 1, 1)[0];
                if (type.Equals('i'))
                {
                    int a = BitConverter.ToInt32(data, 2);
                    int b = BitConverter.ToInt32(data, 6);
                    int sum = a + b;
                    packet = new Packet(commandID, type, sum);
                    SendToClient(packet, sender);
                }
                else if (type.Equals('f'))
                {
                    float a = BitConverter.ToSingle(data, 2);
                    float b = BitConverter.ToSingle(data, 6);
                    float sum = a + b;
                    packet = new Packet(commandID, type, sum);
                    SendToClient(packet, sender);
                }
                else
                {
                    packet = new Packet(0, "Type must be both integer or float");
                    SendToClient(packet, sender);
                }

            }
        }

        public void Subtract(byte[] data, EndPoint sender)
        {
            if (data != null && data.Length == 10)
            {
                byte commandID = data[0];
                Packet packet = new Packet();

                char type = Encoding.UTF8.GetChars(data, 1, 1)[0];
                if (type.Equals('i'))
                {
                    int a = BitConverter.ToInt32(data, 2);
                    int b = BitConverter.ToInt32(data, 6);
                    int sub = a - b;
                    packet = new Packet(commandID, type, sub);
                    SendToClient(packet, sender);
                }
                else if (type.Equals('f'))
                {
                    float a = BitConverter.ToSingle(data, 2);
                    float b = BitConverter.ToSingle(data, 6);
                    float sub = a - b;
                    packet = new Packet(commandID, type, sub);
                    SendToClient(packet, sender);
                }
                else
                {
                    packet = new Packet(0, "Type must be both integer or float");
                    SendToClient(packet, sender);
                }

            }
        }

        public void Multiply(byte[] data, EndPoint sender)
        {
            if (data != null && data.Length == 10)
            {
                byte commandID = data[0];
                Packet packet = new Packet();

                char type = Encoding.UTF8.GetChars(data, 1, 1)[0];
                if (type.Equals('i'))
                {
                    int a = BitConverter.ToInt32(data, 2);
                    int b = BitConverter.ToInt32(data, 6);
                    int mul = a * b;
                    packet = new Packet(commandID, type, mul);
                    SendToClient(packet, sender);
                }
                else if (type.Equals('f'))
                {
                    float a = BitConverter.ToSingle(data, 2);
                    float b = BitConverter.ToSingle(data, 6);
                    float mul = a * b;
                    packet = new Packet(commandID, type, mul);
                    SendToClient(packet, sender);
                }
                else
                {
                    packet = new Packet(0, "Type must be both integer or float");
                    SendToClient(packet, sender);
                }

            }
        }

        public void Divide(byte[] data, EndPoint sender)
        {
            if (data != null && data.Length == 10)
            {
                byte commandID = data[0];
                Packet packet = new Packet();

                char type = Encoding.UTF8.GetChars(data, 1, 1)[0];
                if (type.Equals('i'))
                {
                    int a = BitConverter.ToInt32(data, 2);
                    int b = BitConverter.ToInt32(data, 6);
                    int division = 0;
                    try
                    {
                    division = a / b;
                    }
                    catch (DivideByZeroException)
                    {                       
                        packet = new Packet(0, "The second number cannot be 0");
                        SendToClient(packet, sender);
                    }
                    packet = new Packet(commandID, type, division);
                    SendToClient(packet, sender);
                }
                else if (type.Equals('f'))
                {
                    float a = BitConverter.ToSingle(data, 2);
                    float b = BitConverter.ToSingle(data, 6);
                    float division = 0;
                    try
                    {
                        division = a / b;
                        if (float.IsNaN(division) || float.IsInfinity(division))
                            throw new DivideByZeroException();
                    }
                    catch (DivideByZeroException)
                    {
                        packet = new Packet(0, "The second number cannot be 0");
                        SendToClient(packet, sender);
                    }
                    packet = new Packet(commandID, type, division);
                    SendToClient(packet, sender);
                }
                else
                {
                    packet = new Packet(0, "Type must be both integer or float");
                    SendToClient(packet, sender);
                }

            }
        }

        #endregion

        public GameServer(IGameTransport gameTransport, IMonotonicClock clock)
        {
            this.transport = gameTransport;
            this.clock = clock;
            clientsTable = new Dictionary<EndPoint, GameClient>();
            commandsTable = new Dictionary<byte, GameCommand>();
            commandsTable[0] = ReverseString;
            commandsTable[1] = Add;
            commandsTable[2] = Subtract;
            commandsTable[3] = Multiply;
            commandsTable[4] = Divide;
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

            if (data != null)
            {
                byte gameCommand = data[0];
                if (commandsTable.ContainsKey(gameCommand))
                {
                    commandsTable[gameCommand](data, sender);
                }
            }

            foreach (GameClient client in clientsTable.Values)
            {
                client.Process();
            }
        }

        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public void RemoveIndexCharFromString(ref string text, int index)
        {
            StringBuilder sb = new StringBuilder(text);
            sb.Remove(index, 1);
            text = sb.ToString();
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
