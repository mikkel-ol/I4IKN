using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcp
{
	class file_client
	{
        // Nested class for handling socket data
        class SocketData
        {
            public const int BUFSIZE = 1000; // Maximum packet size

            public Socket sock;
            public byte[] buffer = new byte[BUFSIZE];
            public int fileSize = 0;
        }

        const int PORT = 9000;

        private IPAddress ipAddress;
        private IPEndPoint remoteEndPoint;

		// Event signals
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone 	= new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private file_client (string[] args)
		{
			// Getting path for file to request
			Console.Write("Request file: ");
			string path = Console.ReadLine();

			// Getting IP-address
			Console.Write("Input IP-address to connect to: "); 
			ipAddress = IPAddress.Parse(Console.ReadLine());
			remoteEndPoint = new IPEndPoint(ipAddress, PORT);
			
			Console.Write("Opening client socket connection.. ");
			
			try {
				// Create socket 
				Socket clientSocket = new Socket(
					ipAddress.AddressFamily,
					SocketType.Stream, 
					ProtocolType.Tcp
				);

				// Connect to the remote endpoint
				clientSocket.BeginConnect(remoteEndPoint,
					new AsyncCallback(ConnectedToServerCallback), clientSocket);
				connectDone.WaitOne();

				// Send data to the remote device.  
				Send(clientSocket, path);
				sendDone.WaitOne();

				// Receive file
				Receive(clientSocket);
				receiveDone.WaitOne();

				// Release socket
				clientSocket.Shutdown(SocketShutdown.Both);
				clientSocket.Close();

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
			}
        }

		private void ConnectedToServerCallback(IAsyncResult ar)
		{
            try
            {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket) ar.AsyncState;

                // Complete the connection.  
                socket.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    socket.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

		private void Receive(Socket socket)
		{
			try {
				// Create object to receive data to
				SocketData data = new SocketData();
				data.sock = socket;

                // Get file size
                byte[] fileSize = new byte[SocketData.BUFSIZE];
                int recv = socket.Receive(fileSize);
                data.fileSize = Int32.Parse(Encoding.ASCII.GetString(fileSize, 0, recv));

                Console.Write("Receiving file.. ");
				data.sock.BeginReceive(data.buffer, 0, SocketData.BUFSIZE, 0,
					new AsyncCallback(ReceiveCallback), data);

			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			try {
                SocketData data = (SocketData) ar.AsyncState;
                Socket handler = data.sock;

                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // Check for all data read
                    if (bytesRead <= SocketData.BUFSIZE) {
						Console.WriteLine("Done.");

                        File.WriteAllBytes("/Users/mikkel/Desktop/test-receive.txt", data.buffer);

                    } else {
                        // Not all data received. Get more.
                        handler.BeginReceive(data.buffer, 0, SocketData.BUFSIZE, 0,
                            new AsyncCallback(ReceiveCallback), data);
                    }
                }
				
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void Send(Socket socket, string text)
		{
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(text);

            // Begin sending the data to the remote device.  
            socket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), socket);
        }

		private void SendCallback(IAsyncResult ar)
		{
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Requested file.");

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts..");
			new file_client(args);
		}
	}
}
