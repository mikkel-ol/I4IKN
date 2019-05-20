using System;
using System.IO;
using System.Collections;
using System.Text;

using System.Linq;

namespace Server
{
    class Program
    {
        const int MAX_SIZE = 1000;
        static Server server;

        static void Main(string[] args)
        {
            WriteLine("Server started", Color.GREEN);
            WriteLine("");

            Write("Input serial device (eg. COM3): ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string port = Console.ReadLine();
            Console.ResetColor();

            try
            {
                server = new Server(port, MAX_SIZE);
            }
            catch (PortException)
            {
                WriteLine("ERROR. COULD NOT OPEN PORT. EXITING", Color.RED);
                return;
            }

            WriteLine($"Server created on port \"{port.ToUpper()}\"");

            while (true)
            {
                Loop();
            }
        }

        static void Loop()
        {
            Write("Waiting for data.. ");

            var buffer = new ArrayList();
            server.Receive(ref buffer);

            WriteLine("Request received.");

            // Get byte array from received request
            var arr = (byte[])buffer.ToArray(typeof(byte));

            // Convert to string and get file size
            Write("Reading file info.. ");
            var fileName = Encoding.ASCII.GetString(arr, 0, arr.Length);
            var fileSize = new FileInfo(
                    Path.Combine(Environment.CurrentDirectory, fileName)
                ).Length;
            WriteLine("Done");

            // Send file size
            Write("Sending file size.. ");
            server.Send(Encoding.ASCII.GetBytes(fileSize.ToString()));
            WriteLine("Done");

            // Read file into byte array
            var file = File.ReadAllBytes(fileName);

            // Send file
            Write("Sending file.. ");
            server.Send(file);
            WriteLine("Done");

            WriteLine("\nClient handled. Restarting..\n", Color.GREEN);
        }

        #region helper methods
        static void Write(string msg, Color c = Color.DEFAULT)
        {
            switch (c)
            {
                case Color.RED:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case Color.GREEN:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                case Color.BLUE:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;

                case Color.GRAY:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                default:
                    break;
            }

            Console.Write(msg);
            Console.ResetColor();
        }

        static void WriteLine(string msg, Color c = Color.DEFAULT)
        {
            switch (c)
            {
                case Color.RED:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case Color.GREEN:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                case Color.BLUE:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;

                case Color.GRAY:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                default:
                    break;
            }

            Console.WriteLine(msg);
            Console.ResetColor();
        }

        enum Color
        {
            RED,
            GREEN,
            BLUE,
            GRAY,
            DEFAULT
        }

        #endregion
    }
}
