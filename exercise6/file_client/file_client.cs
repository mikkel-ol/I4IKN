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
        int bytesReceived = 0;

        Socket sender;

        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint remoteEndPoint;

        private file_client()
        {
            // Define ip address and endpoint as local computer
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            remoteEndPoint = new IPEndPoint(ipAddress, PORT);

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

                // Receive file
                ReceiveFile();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CreateSocket()
        {
            Console.Write("Creating socket.. \t");

            sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            WriteInColor("GREEN", "Done.");
        }

        private void ConnectToServer()
        {
            Console.Write("Connecting to server.. \t");
            sender.Connect(remoteEndPoint);
            WriteInColor("GREEN", $"{sender.RemoteEndPoint.ToString()}");
        }

        private void GetFileFromUser()
        {
            Console.Write("Path to file: \t\t");
            path = Console.ReadLine();

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
                    "ERROR. FILE NOT FOUND ON SERVER." +
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
            // "/Desktop/test.txt"
            string fileExtension = Path.GetExtension(file); // ".txt"
            string fileName = file.Substring(0, file.Length - fileExtension.Length); // "test"

            string fileToWrite = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName + "_received" + fileExtension;

            int i = 1;
            while (File.Exists(fileToWrite)) {
                fileToWrite = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName + "_received_" + i + fileExtension;
                i++;

                // Sanity check
                if (i > 100) {
                    WriteInColor("RED", "ERROR. CANNOT WRITE FILE.");
                    return;
                }
            }

            int fileSize = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 3, bytesReceived - 3));

            // Retrieve file
            sender.Blocking = true;
            buffer = new Byte[fileSize];

            bytesReceived = sender.Receive(buffer);
            while ((bytesReceived % 1000) == 0)
            { // More data
                bytesReceived += sender.Receive(buffer);
            }
            File.WriteAllBytes(fileToWrite, buffer);

            WriteInColor("GREEN", "Done.");
            Console.WriteLine($"\nFile saved as \"{fileToWrite}\"");
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