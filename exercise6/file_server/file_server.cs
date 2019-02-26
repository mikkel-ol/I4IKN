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

        private file_server()
        {
            byte[] buffer = new Byte[BUFSIZE];
			string filePath = null;

			// Define ip address and endpoint as local computer
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

            // Create a TCP/IP socket
			Console.Write("Creating socket.. ");
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
			Console.WriteLine("Done.");

        	// Bind the socket to the local endpoint and listen for incoming connections
            try {
				Console.Write("Listening for connections.. ");
                listener.Bind(localEndPoint);
                listener.Listen(MAXCONN);

                // Start listening for connections.  
                while (true)
                {
                    // Blocking
                    Socket handler = listener.Accept();
					Console.WriteLine("Client connected.");

					// Assume first packet is path
					Console.Write("Getting file path.. ");
					handler.Blocking = true;
					int bytesReceived = handler.Receive(buffer);
                    filePath = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
					Console.WriteLine("File path received.");

					// Check if file exist
					Console.Write("Checking if file exists.. ");
					if (!File.Exists(filePath)) {
						Console.ForegroundColor = ConsoleColor.DarkRed;
						Console.WriteLine("FILE NOT FOUND. SENDING ERROR CODE.");
						Console.ResetColor();

						// Send error code
						handler.Send(Encoding.ASCII.GetBytes("404"));

					} else {
						// File exists
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

					Console.Write("\nWaiting for new connection.. ");
                }

            } catch (Exception e) {
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts..\n");
            new file_server();
        }
    }
}
