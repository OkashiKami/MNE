using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MNE
{
    class Program
    {
        private static Socket _listener;
        private static List<Client> _clients = new List<Client>();
        static void Main(string[] args)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 22605));
            if (!_listener.IsBound) return;
            _listener.Listen(1000);
            Log("You can press <color=yellow>Any Key</color> at any time to <color=red>Exit</color> this application.");
            Log($"Server <color=green>Started</color> waiting for connections...");
            _listener.BeginAccept(new AsyncCallback(OnClientAccepted), null);

            byte[] buffer;
            using (var packet = new Packet())
            {
                packet.Write(PacketHeader.Welcome);
                packet.Write("Thanks for joining our server.");

                buffer = packet;
            }

            using(var packet = new Packet(buffer))
            {
                Log($"<color={ConsoleColor.Gray}>================================================");
                Log($"<color={ConsoleColor.Cyan}>Header</color>:   {packet.ReadHeader}");
                Log($"<color={ConsoleColor.Yellow}>Message</color>:  {packet.ReadString}");
                Log($"<color={ConsoleColor.Gray}>================================================");
            }



            Console.ReadKey();
            _listener.Dispose();
        }

        private static void OnClientAccepted(IAsyncResult ar)
        {
            var _tmpClient = _listener.EndAccept(ar);
            var _client = new Client(_tmpClient);
            _client.onDisconnected += (a) =>
            {
                Log($"[<color=red>-</color>]Client {a} disconnected!");
                _clients.RemoveAt(_clients.IndexOf(a));
            };

            Log($"[<color=green>+</color>] Client {_client} connected!");
            _clients.Add(_client);

            if (_listener.IsBound)
                _listener.BeginAccept(new AsyncCallback(OnClientAccepted), null);
        }

        private static void Log(string data)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($" #  ");
            Console.ForegroundColor = ConsoleColor.White;
            var chars = data.ToCharArray();
            for (int a = 0; a < chars.Length - 1; a++)
            {
                if (chars[a].Equals('<'))
                {
                    var sb = new StringBuilder();
                    for (int b = a + 1; b < chars.Length; b++)
                    {
                        if (chars[b].Equals('>'))
                        {
                            a = b + 1;
                            break;
                        }
                        sb.Append(chars[b]);
                    }
                    var result = sb.ToString();
                    if (result == "/color")
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                    {
                        var _result = result.Split('=')[1];
                        var _color = ConsoleColor.White;
                        try { _color = Enum.Parse<ConsoleColor>(_result, true); }
                        catch { _color = ConsoleColor.White; }

                        Console.ForegroundColor = _color;
                    }
                }
                if (a < chars.Length)
                    Console.Write(chars[a]);
            }



            Console.Write("\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
