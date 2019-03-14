using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace client
{
    class Client
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;

        byte[] buffer = new Byte[BUFSIZE];

        char command;

        UdpClient udp;
        IPEndPoint remoteEndPoint;

        string ipAddress;

        static void Main(string[] args)
        {
            WriteInColor("MAGENTA", "\n" +
                "CLIENT IS STARTING UP.."
            + "\n");

            new Client();   
        }

        private Client()
        {
            // Get IP to server
            GetIpFromUser();

            // Create a UDP client
            CreateClient();

            // Connect to end point
            ConnectToServer();

            // Get command
            GetCommandFromUser();

            // Send command
            udp.Send(new byte[] { Convert.ToByte(command) }, 1);
        }

        private void GetIpFromUser()
        {
            Console.Write("(press Enter for ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("localhost");
            Console.ResetColor();
            Console.WriteLine(")");

            Console.Write("IP to server: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;

            // Set IP to localhost if no input
            string input = Console.ReadLine();
            ipAddress = input.Length > 0 ? input : "127.0.0.1";

            Console.ResetColor();

            Console.WriteLine("");
        }

        private void CreateClient()
        {
            Console.Write("Creating UDP client.. \t\t");

            udp = new UdpClient();
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), PORT);

            WriteInColor("GREEN", "Done.");
        }

        private void ConnectToServer()
        {
            Console.Write("Connecting to server.. \t\t");

            udp.Connect(remoteEndPoint);

            WriteInColor("GREEN", "Done.");
        }

        private void GetCommandFromUser()
        {
            Console.WriteLine("");

            Console.Write("(Uptime: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("u/U");
            Console.ResetColor();
            Console.WriteLine(")");

            Console.Write("(Load average: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("l/L");
            Console.ResetColor();
            Console.WriteLine(")");

            Console.Write("Input request: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;

            // Get command
            string input = Console.ReadLine();

            // Check if it matches
            Regex r = new Regex("(u|U|l|L)");
            Match m = r.Match(input);

            // If not matching, try again
            while (input.Length != 1 || !m.Success) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(
                    "\n" +
                    "ERROR - COMMAND NOT FOUND."
                );
                Console.ResetColor();
                Console.Write("Input request: ");
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                input = Console.ReadLine();
                m = r.Match(input);
            }

            // Save input command
            command = input[0];

            Console.ResetColor();
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
