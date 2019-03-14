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

        UdpClient udp;

        IPEndPoint remoteEndPoint;

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
            try {
                // Create UDP client
                CreateClient();

                while (true) 
                {
                    // Get data (blocking)
                    ReceiveData();
                    WriteInColor("GREEN", "Data received");

                    // Receive command from client
                    if (!GetCommand()) WriteInColor("RED", "NO COMMAND RECEIVED");

                    // Handle command
                    else if (!HandleCommand()) WriteInColor("RED", "UNKNOWN COMMAND RECEIVED");

                    // Start over
                    WriteInColor("CYAN", "CLIENT HANDLED. RESTARTING UDP CLIENT");
                    CreateClient();
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void CreateClient()
        {
            Console.Write("Creating UDP client.. \t\t");

            udp = new UdpClient(PORT);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            WriteInColor("GREEN", "Done.");
        }

        private void ReceiveData()
        {
            try 
            {
                // Wait until data is received
                Console.Write("Ready to receive data.. \t");
                buffer = udp.Receive(ref remoteEndPoint);
            }
            catch (Exception) 
            {

            }
        }

        private bool GetCommand()
        {
            // Wait for command from client
            Console.Write("Getting command.. \t\t");

            string received = Encoding.ASCII.GetString(buffer);

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
            udp.Connect(remoteEndPoint);

            var content = File.ReadAllBytes(uptime);

            udp.Send(content, content.Length, remoteEndPoint);
        }

        private void SendLoadAvg()
        {
            udp.Connect(remoteEndPoint);

            var content = File.ReadAllBytes(loadavg);

            udp.Send(content, content.Length, remoteEndPoint);
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
