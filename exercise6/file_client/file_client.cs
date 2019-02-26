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

        private file_client()
        {
            byte[] buffer = new Byte[BUFSIZE];

            // Define ip address and endpoint as local computer
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, PORT);

            // Create a TCP/IP socket
            Console.Write("Creating socket.. ");
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Done.");

            try {
                Console.Write("Connecting to server.. ");
                sender.Connect(remoteEndPoint);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Connected to {sender.RemoteEndPoint.ToString()}");
                Console.ResetColor();

                // Get file path
                Console.Write("Path to file: ");
                string path = Console.ReadLine();

                string fileName = (path.LastIndexOf('/') == 0 ? path : path.Substring(path.LastIndexOf('/') + 1));
                Console.Write($"Retrieving file {fileName}.. ");

                
                // Request file
                sender.Send(Encoding.ASCII.GetBytes(path));

                // Get reply
                int bytesReceived = sender.Receive(buffer);
                string statusCode = Encoding.ASCII.GetString(buffer, 0, 3);

                // Was file found?
                if (statusCode == "404") {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(
                        "ERROR. FILE NOT FOUND." +
                        "\n\n" +
                        "EXITING." +
                        "\n"
                    );
                    Console.ResetColor();

                    return;
                }

                // Extract file size from reply
                string fileSizeString = Encoding.ASCII.GetString(buffer, 3, bytesReceived-3);
                int fileSize = Convert.ToInt32(fileSizeString);

                // Retrieve file
                sender.Blocking = true;
                buffer = new Byte[fileSize];
                string pathWriteReceived = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/received-" + fileName;

                bytesReceived = sender.Receive(buffer);
                while ((bytesReceived % 1000) == 0) { // More data
                    bytesReceived += sender.Receive(buffer);
                }
                File.WriteAllBytes(pathWriteReceived, buffer);

                Console.WriteLine("Done.");
                Console.WriteLine($"\nFile saved to {pathWriteReceived}.");

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Client starts..\n");
            new file_client();
        }
    }
}
