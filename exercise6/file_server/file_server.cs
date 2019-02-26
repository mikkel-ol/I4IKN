using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
    class file_server
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;
		const int MAXCONN = 1; // Not working ..
        
		byte[] buffer = new Byte[BUFSIZE];
		string filePath;

		Socket listener, handler;

        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        private file_server()
        {
			// Define ip address and endpoint as local computer
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, PORT);

			try {
            	// Create a TCP/IP socket and start listening
				CreateSocket();

                // Accept connections forever
                while (true)
                {
                    // Blocking
                    handler = listener.Accept();
                    Console.WriteLine("Client connected.");

					// Get file path
                    GetFilePath();

					// Check if file exist
					Console.Write("Checking if file exists.. ");
					if (!File.Exists(filePath)) {
						// Send error code
						FileNotFound();

					} else { // File exists, send file
                        SendFile();
					}

					Console.Write("\nWaiting for new connection.. ");
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

		private void CreateSocket()
		{
            Console.Write("Creating socket.. ");

            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Done.");

            Console.Write("Listening for connections.. ");
            listener.Bind(localEndPoint);
            listener.Listen(MAXCONN);
		}

		private void GetFilePath()
		{
            // Assume first packet is path
            Console.Write("Getting file path.. ");
            handler.Blocking = true;
            int bytesReceived = handler.Receive(buffer);
            filePath = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("File path received.");
		}

		private void FileNotFound()
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("FILE NOT FOUND. SENDING ERROR CODE.");
            Console.ResetColor();

            // Send error code
            handler.Send(Encoding.ASCII.GetBytes("404"));		
		}

		private void SendFile()
		{
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("File found.");
            Console.ResetColor();

            // Send file size
            Console.Write("Sending file size.. ");
            byte[] fileSize = Encoding.ASCII.GetBytes("200" + new FileInfo(filePath).Length.ToString());
            handler.Send(fileSize);
            Console.WriteLine("Done.");

            // Send file
            Console.Write("Sending file.. ");
            handler.SendFile(filePath);
            Console.WriteLine("Done.");
		}

        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts..\n");
            new file_server();
        }
    }
}
