using System;
using System.Collections;
using System.Text;

using System.Linq;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Server started", Color.GREEN);
            WriteLine("");

            Write("Input serial device: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string port = "dev/ttys001"; //Console.ReadLine();
            Console.ResetColor();

            var server = new Server(port);

            Console.WriteLine($"Server created on port \"{port}\"");

            var testArray = new byte[1];
            testArray[0] = (byte) 'B';

            server.Send(testArray);
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
