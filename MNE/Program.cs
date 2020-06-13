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
            Log("Hello World!");
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 22605));
            if (!_listener.IsBound) return;
            _listener.Listen(1000);
            Log($"Server <color=green>Started</color> waiting for connections...");
            _listener.BeginAccept(new AsyncCallback(OnClientAccepted), null);

            Log("Press anykey to <color=red>Exit</color> this application.");
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
            for (int a = 0; a < chars.Length; a++)
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

                Console.Write(chars[a]);
            }



            Console.Write("\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
