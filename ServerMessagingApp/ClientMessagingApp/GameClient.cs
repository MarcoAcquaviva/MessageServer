using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMessagingApp
{
    public class GameClient
    {
        private string address;
        private int port;
        Socket socket;
        private IPEndPoint endPoint;

        bool sendData;

        private Dictionary<uint, GameCommand> commandsTable;
        private delegate void GameCommand(byte[] data, EndPoint sender);

        #region Client Command Methods

        private void PrintString(byte[] data, EndPoint sender)
        {
            if (data != null)
            {
                string output = Encoding.UTF8.GetString(data);
                RemoveIndexCharFromString(ref output, 0);
                Console.WriteLine(output);
                Console.ReadLine();
            }
        }

        private void PrintNumber(byte[] data, EndPoint sender)
        {
            if (data != null)
            {
                char type = Encoding.UTF8.GetChars(data, 1, 1)[0];
                if (type == 'i')
                {
                    int resultInteger = BitConverter.ToInt32(data, 2);
                    Console.WriteLine("The result of operation is {0}", resultInteger);
                    Console.ReadLine();
                }
                else if (type == 'f')
                {
                    //TO DO check why it doesn't split number with point ex: 1.5 + 1.5 = 30
                    float resultFloat = BitConverter.ToSingle(data, 2);
                    Console.WriteLine("The result of operation is {0}", resultFloat);
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Type Not Found - Contact Server Administrator");
                }
            }
        }

        #endregion

        public GameClient(string address, int port)
        {
            this.address = address;
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            commandsTable = new Dictionary<uint, GameCommand>();
            commandsTable[0] = PrintString;
            commandsTable[1] = PrintNumber;
        }

        private void SendToServer(Packet packet, EndPoint endPoint)
        {
            socket.SendTo(packet.GetData(), endPoint);
        }

        public void Run()
        {
            Console.WriteLine("Welcome Client");
            while (true)
            {
                SingleStep();
            }
        }

        public void SingleStep()
        {
            sendData = AskDataInput();
            if (sendData)
            {
                SendData();
            }

            ReciveData();


        }

        private bool AskDataInput()
        {
            Console.WriteLine("Do you wanna send data?(y/n)");
            string answer = Console.ReadLine();
            if (answer == "y")
                return true;
            else
                return false;
        }

        private void SendData()
        {
            byte commandID;
            Packet packet;
            do
            {
                Console.WriteLine("Insert operation code you want perform");
                Console.WriteLine("0 - Reverse string");
                Console.WriteLine("1 - Addition");
                Console.WriteLine("2 - Subtraction");
                Console.WriteLine("3 - Multiply");
                Console.WriteLine("4 - Division");
                commandID = byte.Parse(Console.ReadLine());
                if (commandID <= 0 && commandID >= 4)
                    Console.WriteLine("Error Code");

            } while (commandID < 0 || commandID > 4);

            switch (commandID)
            {
                case 0:
                    Console.WriteLine("Write String that has been to reverse");
                    string answer = Console.ReadLine();
                    packet = new Packet(commandID, answer);
                    SendToServer(packet, endPoint);
                    break;

                default:
                    Console.WriteLine("What kind of type are numbers?(i/f)");
                    char type = char.Parse(Console.ReadLine().ToLower());
                    switch (type)
                    {
                        case 'i':
                            //TO DO try catch for input error
                            Console.WriteLine("Insert first number");
                            int a = int.Parse(Console.ReadLine());
                            Console.WriteLine("Insert second number");
                            int b = int.Parse(Console.ReadLine());

                            packet = new Packet(commandID, type, a, b);
                            SendToServer(packet, endPoint);

                            break;
                        case 'f':
                            //TO DO try catch for input error
                            Console.WriteLine("Insert first number");
                            string answerfloat = Console.ReadLine().Replace('.', ',');
                            float fa = float.Parse(answerfloat);
                            Console.WriteLine("Insert second number");
                            answerfloat = Console.ReadLine().Replace('.', ',');
                            float fb = float.Parse(answerfloat);

                            packet = new Packet(commandID, type, fa, fb);
                            SendToServer(packet, endPoint);
                            break;
                    }

                    break;
            }

        }

        private void ReciveData()
        {
            //Recive Data
            byte[] data = new byte[256];
            int rlen = -1;
            try
            {
                rlen = socket.Receive(data);
            }
            catch
            {

            }

            if (rlen > 0)
            {

                if (data != null)
                {
                    byte gameCommand = data[0];
                    if (commandsTable.ContainsKey(gameCommand))
                    {
                        commandsTable[gameCommand](data, endPoint);
                    }
                }
            }
        }

        public void RemoveIndexCharFromString(ref string text, int index)
        {
            StringBuilder sb = new StringBuilder(text);
            sb.Remove(index, 1);
            text = sb.ToString();
        }
    }
}
