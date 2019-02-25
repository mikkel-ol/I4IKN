using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcp
{
	class file_server
	{
		// Nested class for handling socket data
		class SocketData {
			public const int BUFSIZE = 1000; // Maximum packet size

			public Socket sock;
            public byte[] buffer = new byte[BUFSIZE];
			public StringBuilder sb = new StringBuilder();
		}

		const int PORT = 9000;
		const int MAXCONNECTIONS = 1;
		
		static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
		static IPAddress ipAddress = ipHostInfo.AddressList[0];
		static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

        static ManualResetEvent eventSignal = new ManualResetEvent(false);

        private file_server ()
		{
			// Open server socket connection and listen for clients
			Console.Write("Opening server socket connection.. ");
			Socket serverSocket = new Socket(
				ipAddress.AddressFamily,
				SocketType.Stream,
				ProtocolType.Tcp
			);
			Console.WriteLine("Done");

            try {
				// Listen for connections
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(MAXCONNECTIONS);

                while (true) {
                    // Set the event to nonsignaled state.  
                    eventSignal.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    serverSocket.BeginAccept(
                        new AsyncCallback(AcceptConnectionCallback),
                        serverSocket);
						
                    // Wait until a connection is made before continuing.  
                    eventSignal.WaitOne();
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }	
		}

		private void AcceptConnectionCallback(IAsyncResult ar)
		{
			Console.Write("Connection made. ");
            // Signal the main thread to continue.  
            eventSignal.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

			// Create data handler object
			SocketData data = new SocketData();
			data.sock = handler;

			Console.WriteLine("Beginning to receive data from client..");
            handler.BeginReceive(data.buffer, 0, SocketData.BUFSIZE, 0,
                new AsyncCallback(ReadCallback), data);
        }

		private void ReadCallback(IAsyncResult ar)
		{
			SocketData data = (SocketData) ar.AsyncState;
			Socket handler = data.sock;

            String content = String.Empty;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.  
                data.sb.Append(Encoding.ASCII.GetString(data.buffer, 0, bytesRead));

                content = data.sb.ToString();

				// Check for all data read
				if (bytesRead <= SocketData.BUFSIZE) {
					// Send file to client
					SendFile(content, handler);

				} else {
					// Not all data received. Get more.
					handler.BeginReceive(data.buffer, 0, SocketData.BUFSIZE, 0,
						new AsyncCallback(ReadCallback), data);
				}
            }
        }

		private void SendFile(String fileName, Socket socket)
		{
			if (!File.Exists(fileName)) {



			} else {
				// Send file size
				byte[] filesize = Encoding.ASCII.GetBytes(new FileInfo(fileName).Length.ToString());
				socket.Send(filesize);

				// Send file
				socket.SendFile(fileName);
			}
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts..");
			new file_server();
		}
	}
}
