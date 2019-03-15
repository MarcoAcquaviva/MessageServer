using System;
using NUnit.Framework;
using System.Text;

namespace ServerMessagingApp.Test
{
    public class TestGameServer
    {
        private FakeTransport transport;
        private FakeClock clock;
        private GameServer server;

        [SetUp]
        public void SetupTests()
        {
            transport = new FakeTransport();
            clock = new FakeClock();
            server = new GameServer(transport, clock);
        }

        [Test]
        public void TestZeroNow()
        {
            Assert.That(server.Now, Is.EqualTo(0));
        }

        [Test]
        public void TestClientsOnStart()
        {
            Assert.That(server.NumClients, Is.EqualTo(0));
        }

        [Test]
        public void TestRecivePacket()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet,"test",0);
            Assert.That(transport.ServerQueueCount, Is.EqualTo(1));
        }

        [Test]
        public void TestSendPacketToClient()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet,"test",0);
            server.SingleStep();
            Assert.That(transport.ClientQueueCount, Is.EqualTo(1));
        }

        [Test]
        public void TestClientRecivePacket()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            transport.ClientDequeue();
            Assert.That(transport.ClientQueueCount, Is.EqualTo(0));
        }
        
        [Test]
        public void TestPacketHasNotSent()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            transport.ClientDequeue();
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            Assert.That(transport.ClientQueueCount, Is.EqualTo(1));
        }

        [Test]
        public void TestDataRecived()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet, "test", 0);
            FakeData data = transport.ServerDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);
            Assert.That(text, Is.EqualTo("ciao"));
        }

        [Test]
        public void TestDataSent()
        {
            Packet packet = new Packet(0,"ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);
            Assert.That(text, Is.EqualTo("oaic"));
        }

        
        [Test]
        public void TestServerAddPacketLength()
        {
            Packet packet = new Packet(1,'i',1,1);
            transport.ClientEnqueue(packet, "test", 0);
            FakeData data = transport.ServerDequeue();
            Assert.That(data.data.Length, Is.EqualTo(10));
        }

                
        [Test]
        public void TestClientAddPacketLength()
        {
            Packet packet = new Packet(1,'i',1,1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();     
            Assert.That(data.data.Length, Is.EqualTo(6));
        }

                        
        [Test]
        public void TestClientReciveCorrectSum()
        {
            Packet packet = new Packet(1,'i',1,1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            int sum = BitConverter.ToInt32(data.data, 2);
            Assert.That(sum, Is.EqualTo(2));
        }


    }
}
