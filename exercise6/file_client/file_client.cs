using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
    class file_client
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;
        const string ACK = "200";
        const string SUCC = "200";
        const string NOTFOUND = "404";

        byte[] buffer = new Byte[BUFSIZE];
        string path, file;
        int bytesReceived;

        Socket sender;

        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint remoteEndPoint;

        private file_client()
        {
            // Get IP
            GetIpFromUser();

            // Create a TCP/IP socket
            CreateSocket();

            try
            {
                // Connect to file server
                ConnectToServer();

                // Ask for file
                GetFileFromUser();

                // Request file from server
                sender.Send(Encoding.ASCII.GetBytes(path));

                // Check status code
                if (!IsFileOnServer()) return;

                // Receive and save file
                ReceiveFile();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            string userIP = Console.ReadLine();
            ipAddress = userIP.Length > 0 ? IPAddress.Parse(userIP) : IPAddress.Parse("127.0.0.1");
            remoteEndPoint = new IPEndPoint(ipAddress, PORT);

            Console.ResetColor();

            Console.WriteLine("");
        }

        private void CreateSocket()
        {
            Console.Write("Creating socket.. \t");

            sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            WriteInColor("GREEN", "Done");
        }

        private void ConnectToServer()
        {
            Console.Write("Connecting to server.. \t");

            try {
                sender.Connect(remoteEndPoint);

                WriteInColor("GREEN", $"{sender.RemoteEndPoint.ToString()}");
            } catch (SocketException) {
                // Connection refused
                WriteInColor("RED", $"ERROR - COULD NOT CONNECT TO {ipAddress.ToString()}:{PORT}\n");

                // Retry
                GetIpFromUser();
                ConnectToServer();

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void GetFileFromUser()
        {
            Console.Write("Path to file: \t\t");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            path = Console.ReadLine();
            Console.ResetColor();

            while (path.Length == 0) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("INPUT FILE PATH:\t");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                path = Console.ReadLine();
                Console.ResetColor();
            }

            file = (path.LastIndexOf('/') == 0 ? path : path.Substring(path.LastIndexOf('/') + 1));
            Console.Write($"Retrieving file.. \t");
        }

        private bool IsFileOnServer()
        {
            bytesReceived = sender.Receive(buffer);

            string statusCode = Encoding.ASCII.GetString(buffer, 0, 3);

            if (statusCode == SUCC) {
                sender.Send(Encoding.ASCII.GetBytes(ACK)); // Send ack

                return true;
            }
            else if (statusCode == NOTFOUND)
            {
                sender.Send(Encoding.ASCII.GetBytes(ACK)); // Send ack

                WriteInColor("RED",
                    "ERROR. FILE NOT FOUND ON SERVER" +
                    "\n\n" +
                    "EXITING"
                );

                return false;
            }

            WriteInColor("RED", "AN UNKNOWN ERROR OCCURED");

            return false;
        }

        private void ReceiveFile()
        {
            int fileSize = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 3, bytesReceived - 3));

            // Retrieve file
            sender.Blocking = true;
            buffer = new Byte[fileSize];

            bytesReceived = sender.Receive(buffer);
            while ((bytesReceived % 1000) == 0)
            { // More data
                bytesReceived += sender.Receive(buffer);
            }

            WriteInColor("GREEN", "Done");

            SaveFile(fileSize);
        }

        private void SaveFile(int fileSize)
        {
            // "../Desktop/test.txt"
            string fileExtension = Path.GetExtension(file); // ".txt"
            string fileName = file.Substring(0, file.Length - fileExtension.Length); // "test"

            Console.Write(
                "\n" + 
                "(press Enter for current folder)\n" + 
                "Save file as: "
            );

            // Get output file path
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string outputFileName = Console.ReadLine();
            Console.ResetColor();
            // If no input, set to current folder
            outputFileName = outputFileName.Length > 0 ? outputFileName : Directory.GetCurrentDirectory() + "/" + fileName + fileExtension;

            // Check if user input extension
            if (Path.GetExtension(outputFileName).Length == 0) outputFileName += fileExtension;

            // Append if file exists
            int i = 1;
            string outputFileNameAppend = outputFileName;
            while (File.Exists(outputFileNameAppend))
            {
                outputFileNameAppend = outputFileName.Substring(0, outputFileName.Length - Path.GetExtension(outputFileName).Length) + "_" + i + fileExtension;
                i++;

                // Sanity check
                if (i > 100)
                {
                    WriteInColor("RED", "ERROR. CANNOT WRITE FILE.");
                    return;
                }
            }
            outputFileName = outputFileNameAppend;

            // Save file
            File.WriteAllBytes(outputFileName, buffer);

            Console.WriteLine($"\nFile saved as \"{outputFileName}\"");
        }

        public static void Main(string[] args)
        {
            WriteInColor("MAGENTA", "\n" +
                "CLIENT IS STARTING UP.."
            + "\n");

            new file_client();
        }

        static void WriteInColor(string color, string msg)
        {
            if (color == "GREEN")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            else if (color == "RED")
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            else if (color == "MAGENTA")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            else return;
        }
    }
}