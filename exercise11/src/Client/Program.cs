using System;
using System.Collections;
using System.Text;

using System.Linq;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Client started", Color.GREEN);
            WriteLine("");

            Write("Input serial device: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string port = "COM4"; //Console.ReadLine();
            Console.ResetColor();

            var client = new Client(port);

            WriteLine($"Client created on port \"{port}\"");

            var buffer = new byte[10000];

            client.Receive(ref buffer);

            string converted = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            //WriteLine(converted);
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
