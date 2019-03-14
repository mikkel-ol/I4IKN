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

            // Output answer to user
            ShowAnswer();
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

            Console.Write("(Uptime:       ");
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
            Console.WriteLine("");
        }

        private void ShowAnswer()
        {
            // Receive answer
            var a = udp.Receive(ref remoteEndPoint);
            string answer = Encoding.ASCII.GetString(a);

            if (command == 'u' || command == 'U') {
                Console.Write("Total uptime:\t\t\t");

                string[] split = answer.Split(' ');

                string totalSeconds = PrettyTime(split[0]);
                string cpuIdle = PrettyTime(split[1]);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(totalSeconds);
                Console.ResetColor();


                Console.Write("Total CPU idle time:\t\t");

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(cpuIdle);
                Console.ResetColor();
            }
            else {
                Console.WriteLine("Load average on server:");

                string[] split = answer.Split(' ');

                double oneMin = Convert.ToDouble(split[0]);
                double fiveMin = Convert.ToDouble(split[1]);
                double fifteenMin = Convert.ToDouble(split[2]);
                
                string[] kernel = split[3].Split('/');
                string kernelCurrent = kernel[0];
                string kernelQueue = kernel[1];

                string pid = split[4];

                Console.Write("1 min:\t\t\t\t");
                if (oneMin < .5) Console.ForegroundColor = ConsoleColor.DarkGreen;
                else if (oneMin < 1) Console.ForegroundColor = ConsoleColor.DarkYellow;
                else Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(oneMin);
                Console.ResetColor();

                Console.Write("5 min:\t\t\t\t");
                if (fiveMin < .5) Console.ForegroundColor = ConsoleColor.DarkGreen;
                else if (fiveMin < 1) Console.ForegroundColor = ConsoleColor.DarkYellow;
                else Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(fiveMin);
                Console.ResetColor();

                Console.Write("15 min:\t\t\t\t");
                if (fifteenMin < .5) Console.ForegroundColor = ConsoleColor.DarkGreen;
                else if (fifteenMin < 1) Console.ForegroundColor = ConsoleColor.DarkYellow;
                else Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(fifteenMin);
                Console.ResetColor();

                Console.Write("Kernel, current:\t\t");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(kernelCurrent);
                Console.ResetColor();

                Console.Write("Kernel, scheduled:\t\t");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(kernelQueue);
                Console.ResetColor();

                Console.Write("Most recent PID:\t\t");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(pid);
                Console.ResetColor();
            }
        }

        private string PrettyTime(string input)
        {
            TimeSpan t = TimeSpan.FromSeconds(Convert.ToDouble(input));
            string prettyTime = string.Format("{0:D2}h {1:D2}m {2:D2}s {3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return prettyTime;
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
