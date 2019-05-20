using System;
using System.IO;
using System.Collections;
using System.Text;

using System.Linq;

namespace Client
{
    class Program
    {
        const int MAX_SIZE = 1000;
        
        static void Main(string[] args)
        {
            WriteLine("Client started", Color.GREEN);
            WriteLine("");

            Write("Input serial device (eg. COM3): ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string port = Console.ReadLine();
            Console.ResetColor();

            Client client;

            try
            {
                client = new Client(port, MAX_SIZE);
            }
            catch(PortException)
            {
                WriteLine("ERROR. COULD NOT OPEN PORT. EXITING", Color.RED);
                return;
            }

            WriteLine($"Client created on port \"{port.ToUpper()}\"\n");

            Write("Request file: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string fileName = Console.ReadLine();
            Console.ResetColor();


            Write($"Requesting file \"{fileName}\".. "); 
            client.Send(Encoding.ASCII.GetBytes(fileName));
            WriteLine("Done");


            Write("Getting file size.. ");
            var buffer = new ArrayList();
            client.Receive(ref buffer);

            var arr = (byte[]) buffer.ToArray(typeof(byte));
            var sizeString = Encoding.ASCII.GetString(arr, 0, arr.Length);

            WriteLine("Done");
            WriteLine("Size: " + sizeString);

            var size = Convert.ToInt64(sizeString);
            var count = (int) Math.Ceiling((double) size / MAX_SIZE);

            // Get file
            Write("\nReceiving file.. ");
            
            buffer = new ArrayList();
            for (int i = 0; i < count ; i++)
            {
                client.Receive(ref buffer);
            }
            WriteLine("Done");

            // Save file
            Write("Saving file.. ");

            fileName = "RECEIVED_" + fileName;
            var file = Path.Combine(Environment.CurrentDirectory, fileName);

            arr = (byte[])buffer.ToArray(typeof(byte));
            File.WriteAllBytes(file, arr);

            WriteLine("Done");

            WriteLine("\nRequest handled. Exiting..", Color.GREEN);
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
