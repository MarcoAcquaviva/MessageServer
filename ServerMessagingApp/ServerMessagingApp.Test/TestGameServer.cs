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
            Packet packet = new Packet(0, "ciao");
            transport.ClientEnqueue(packet, "test", 0);
            Assert.That(transport.ServerQueueCount, Is.EqualTo(1));
        }

        [Test]
        public void TestSendPacketToClient()
        {
            Packet packet = new Packet(0, "ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            Assert.That(transport.ClientQueueCount, Is.EqualTo(1));
        }

        [Test]
        public void TestClientRecivePacket()
        {
            Packet packet = new Packet(0, "ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            transport.ClientDequeue();
            Assert.That(transport.ClientQueueCount, Is.EqualTo(0));
        }

        [Test]
        public void TestPacketHasNotSent()
        {
            Packet packet = new Packet(0, "ciao");
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
            Packet packet = new Packet(0, "ciao");
            transport.ClientEnqueue(packet, "test", 0);
            FakeData data = transport.ServerDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);
            Assert.That(text, Is.EqualTo("ciao"));
        }

        [Test]
        public void TestDataSent()
        {
            Packet packet = new Packet(0, "ciao");
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);
            Assert.That(text, Is.EqualTo("oaic"));
        }

        [Test]
        public void TestServerMathsOperationPacketLength()
        {
            Packet packet = new Packet(1, 'i', 1, 1);
            transport.ClientEnqueue(packet, "test", 0);
            FakeData data = transport.ServerDequeue();
            Assert.That(data.data.Length, Is.EqualTo(10));
        }

        [Test]
        public void TestClientMathsOperationPacketLength()
        {
            Packet packet = new Packet(1, 'i', 1, 1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            Assert.That(data.data.Length, Is.EqualTo(6));
        }

        [Test]
        public void TestClientReciveCorrectSumInteger()
        {
            Packet packet = new Packet(1, 'i', 1, 1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            int sum = BitConverter.ToInt32(data.data, 2);
            Assert.That(sum, Is.EqualTo(2));
        }

        [Test]
        public void TestClientReciveCorrectSumFloat()
        {
            Packet packet = new Packet(1, 'f', 1.5f, 1.5f);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            float sum = BitConverter.ToSingle(data.data, 2);
            Assert.That(sum, Is.EqualTo(3.0f));
        }

        [Test]
        public void TestClientReciveErrorPacketOnWrongType()
        {
            Packet packet = new Packet(1, 'd', 1.0f, 1.0f);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();

            FakeData data = transport.ClientDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);

            Assert.That(data.data.Length, Is.Not.EqualTo(6));
            Assert.That(text, Is.EqualTo("Type must be both integer or float"));
        }

        [Test]
        public void TestClientReciveCorrectSubstractIntegerNumbers()
        {
            Packet packet = new Packet(2, 'i', 1, 1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            int sub = BitConverter.ToInt32(data.data, 2);
            Assert.That(sub, Is.EqualTo(0));
        }

        [Test]
        public void TestClientReciveCorrectSubstractFloatNumbers()
        {
            Packet packet = new Packet(2, 'f', 1.0f, 1.0f);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            float sub = BitConverter.ToSingle(data.data, 2);
            Assert.That(sub, Is.EqualTo(0));
        }

        [Test]
        public void TestClientReciveCorrectMultiplyIntegerNumbers()
        {
            Packet packet = new Packet(3, 'i', 1, 1);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            int mul = BitConverter.ToInt32(data.data, 2);
            Assert.That(mul, Is.EqualTo(1));
        }

        [Test]
        public void TestClientReciveCorrectMultiplyFloatNumbers()
        {
            Packet packet = new Packet(3, 'f', 1.0f, 1.0f);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            float mul = BitConverter.ToSingle(data.data, 2);
            Assert.That(mul, Is.EqualTo(1.0f));
        }

        [Test]
        public void TestClientReciveCorrectDivisionIntegerNumbers()
        {
            Packet packet = new Packet(4, 'i', 4, 2);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            int div = BitConverter.ToInt32(data.data, 2);
            Assert.That(div, Is.EqualTo(2));
        }

        [Test]
        public void TestClientReciveCorrectDivisionFloatNumbers()
        {
            Packet packet = new Packet(4, 'f', 4.0f, 2.0f);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();
            FakeData data = transport.ClientDequeue();
            float div = BitConverter.ToSingle(data.data, 2);
            Assert.That(div, Is.EqualTo(2.0f));
        }

        [Test]
        public void TestClientReciveErrorPacketOnDivisionZeroIntegerNumbers()
        {
            Packet packet = new Packet(4, 'i', 4, 0);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();

            FakeData data = transport.ClientDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);

            Assert.That(text, Is.EqualTo("The second number cannot be 0"));
        }

        [Test]
        public void TestClientReciveErrorPacketOnDivisionZeroFloatNumbers()
        {
            Packet packet = new Packet(4, 'f', 4.0f, 0);
            transport.ClientEnqueue(packet, "test", 0);
            server.SingleStep();

            FakeData data = transport.ClientDequeue();
            string text = Encoding.UTF8.GetString(data.data);
            server.RemoveIndexCharFromString(ref text, 0);

            Assert.That(text, Is.EqualTo("The second number cannot be 0"));
        }
    }
}