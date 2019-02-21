using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tcp
{
	class file_server
	{
		class SocketData {
			const int BUFSIZE = 1000;

			public Socket sock = null;
            public byte[] buffer = new byte[BUFSIZE];
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
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(MAXCONNECTIONS);

                while (true) {
                    // Set the event to nonsignaled state.  
                    eventSignal.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
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
            // Signal the main thread to continue.  
            eventSignal.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

			// Create data handler object
			SocketData data = new SocketData();
			data.sock = handler;

            handler.BeginReceive(data.buffer, 0, data.BUFSIZE, 0,
                new AsyncCallback(ConnectionReadCallback), data);
        }

		private void ConnectionReadCallback(IAsyncResult ar)
		{
			SocketData data = (SocketData) ar.AsyncState;
		}

		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			// TO DO Your own code
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server();
		}
	}
}
