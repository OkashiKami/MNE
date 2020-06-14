using System;
using System.Net.Sockets;
using System.Text;

namespace MNE
{
    internal class Client
    {
        private readonly Guid _guid;
        private Socket _client;
        private byte[] _buffer;

        public delegate void OnDisconnected(Client client);
        public event OnDisconnected onDisconnected;
        public delegate void OnConnected(Client client);
        public event OnConnected onConnected;

        public Client(Socket client)
        {
            _guid = Guid.NewGuid();
            _client = client;

            _buffer = new byte[4096];

            _client.ReceiveBufferSize = _buffer.Length;
            _client.SendBufferSize = _buffer.Length;

            onConnected += OnClientConnected;
            onConnected?.Invoke(this);
        }

        private void OnClientConnected(Client client)
        {
            var packet = new Packet();
            packet.Write(PacketHeader.Welcome);
            packet.Write("Thanks for joining our server");
            Send(packet);
        }

        public void Send(byte[] data = default)
        {
            

        }
    }
}