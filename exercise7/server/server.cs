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

        Socket sock;

        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        string uptime = "/proc/uptime";
        string loadavg = "/proc/loadavg";

        static void Main(string[] args)
        {
            WriteInColor("MAGENTA", "\n" +
                "FILE SERVER IS STARTING UP.."
            + "\n");
            
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
                    while (sock.Available == 0) {}

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

            sock = new Socket(ipAddress.AddressFamily,
                SocketType.Dgram, ProtocolType.Udp);

            WriteInColor("GREEN", "Done.");

            Console.Write("Ready to receive data.. \t");

            sock.EnableBroadcast = true;
            sock.Bind(localEndPoint);
        }

        private bool GetCommand()
        {
            // Wait for command from client
            Console.Write("Getting command.. \t\t");
            sock.Blocking = true;
            int bytesReceived = sock.Receive(buffer);

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
            sock.SendFile(uptime);
        }

        private void SendLoadAvg()
        {
            sock.SendFile(loadavg);
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
