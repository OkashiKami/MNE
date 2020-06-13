using System;
using System.Net.Sockets;

namespace MNE
{
    internal class Client
    {
        private readonly Guid _guid;
        private Socket _client;

        public delegate void OnDisconnected(Client client);
        public event OnDisconnected onDisconnected;


        public Client(Socket client)
        {
            _guid = Guid.NewGuid();
            _client = client;
        }
    }
}