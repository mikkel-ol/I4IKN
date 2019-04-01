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
		const int TIMEOUT = 10 * 1000; // ms
        
		byte[] buffer = new Byte[BUFSIZE];
		string filePath;

		Socket listener, handler;

        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        private file_server()
        {
			// Define ip address and endpoint as local computer
            ipAddress = IPAddress.Any;
            localEndPoint = new IPEndPoint(ipAddress, PORT);

			try {
            	// Create a TCP/IP socket and start listening
				CreateSocket();

                // Accept connections forever
                while (true)
                {
                    // Blocking
                    handler = listener.Accept();
                    listener.Close(); // Close listener socket
            
                    WriteInColor("GREEN", "Client connected.");

					// Get file path
                    if (!GetFilePath()) WriteInColor("RED", "NO FILE PATH RECEIVED.");

                    else {
                        // Check if file exist
                        Console.Write("Checking if file exists.. \t");
                        if (!File.Exists(filePath)) {
                            // Send error code
                            FileNotFound();

                        } else { // File exists, send file
                            SendFile();
                        }
                    }

                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(
                        "CLIENT SUCCESFULLY HANDLED. MAKING NEW SOCKET."
                    );
                    Console.ResetColor();
                    Console.WriteLine("");

                    CreateSocket(); // Start listening
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

		private void CreateSocket()
		{
            Console.Write("Creating socket.. \t\t");

            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            WriteInColor("GREEN", "Done.");

            Console.Write("Listening for connections.. \t");
            listener.Bind(localEndPoint);
            listener.Listen(MAXCONN);
		}

		private bool GetFilePath()
		{
            // Assume first packet is path
            Console.Write("Getting file path.. \t\t");
            handler.Blocking = true;
            int bytesReceived = handler.Receive(buffer);
            filePath = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            
            // No path received
            if (filePath.Length == 0) return false;

            WriteInColor("GREEN", "File path received.");
            return true;
		}

		private void FileNotFound()
		{
            WriteInColor("RED", "FILE NOT FOUND. SENDING ERROR CODE.");

            // Send error code
            handler.Send(Encoding.ASCII.GetBytes("404"));		
		}

		private void SendFile()
		{
            WriteInColor("GREEN", "File found.");

            // Send file size
            Console.Write("Sending file size.. \t\t");
            byte[] fileSize = Encoding.ASCII.GetBytes("200" + (new FileInfo(filePath).Length + 1).ToString()); // + 1 due to appended '0'
            handler.Send(fileSize);
            WriteInColor("GREEN", "Done.");

            // Wait for acknowledge
            handler.Blocking = true;
			handler.ReceiveTimeout = TIMEOUT;

			try {
				int bytesReceived = handler.Receive(buffer);
				string ack = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
				
				if (!(ack == "200")) WriteInColor("RED", "ERROR. ACKNOWLEDGE NOT RECEIVED.");
                else
                { // Send file
                    Console.Write("Sending file.. \t\t\t");

                    handler.SendFile(filePath);
                    handler.Send(new Byte['0']); // Handle empty file

                    WriteInColor("GREEN", "Done.");
                }

			} catch (SocketException) {
				WriteInColor("RED", "ERROR. ACKNOWLEDGE NOT RECEIVED.");

			} catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
		}



        public static void Main(string[] args)
        {
            WriteInColor("MAGENTA", "\n" +
				"FILE SERVER IS STARTING UP.."
			+ "\n");

            new file_server();
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
