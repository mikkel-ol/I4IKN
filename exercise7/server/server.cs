using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server
{
    class Server
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;

        byte[] buffer = new Byte[BUFSIZE];
        char command;

        Socket listener;

        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        static void Main(string[] args)
        {
            new Server();
        }

        private Server()
        {
            // Define ip address and endpoint as local computer
            ipAddress = IPAddress.Any;
            localEndPoint = new IPEndPoint(ipAddress, PORT);

            try {
                // Create a TCP/IP socket and start listening
                CreateSocket();

                while (true) 
                {
                    // Wait for client
                    while (listener.Available == 0) {}

                    WriteInColor("GREEN", "Data received");

                    // Receive command from client
                    if (!GetCommand()) WriteInColor("RED", "NO COMMAND RECEIVED");

                    // Handle command
                    else if (!HandleCommand()) WriteInColor("RED", "UNKNOWN COMMAND RECEIVED");

                    // Start over
                    WriteInColor("CYAN", "CLIENT HANDLED. MAKING NEW SOCKET.");
                    CreateSocket(); // Start listening
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void CreateSocket()
        {
            Console.Write("Creating socket.. \t\t");

            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Dgram, ProtocolType.Udp);

            WriteInColor("GREEN", "Done.");

            Console.Write("Ready to receive data.. \t");

            listener.EnableBroadcast = true;
            listener.Bind(localEndPoint);
        }

        private bool GetCommand()
        {
            // Wait for command from client
            Console.Write("Getting command.. \t\t");
            listener.Blocking = true;
            int bytesReceived = listener.Receive(buffer);

            string received = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

            // Nothing received
            if (received.Length == 0) return false;

            command = received[0];

            WriteInColor("GREEN", "Command received.");
            return true;
        }

        private bool HandleCommand()
        {
            // Handle char received
            switch (command)
            {
                case 'U':
                    SendUptime();
                    break;

                case 'u':
                    SendUptime();
                    break;

                case 'L':
                    SendLoadAvg();
                    break;

                case 'l':
                    SendLoadAvg();
                    break;

                default:
                    return false;
            }
            return true;
        }

        private void SendUptime()
        {
            FileStream fs = File.Open("/proc/uptime", FileMode.Open);
        }

        private void SendLoadAvg()
        {

        }

        static void WriteInColor(string color, string msg)
        {
            switch (color) 
            {
                case "GREEN":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                case "RED":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case "MAGENTA":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;

                case "CYAN":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                
                default:
                    break;
            }

            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
