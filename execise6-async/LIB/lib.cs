using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	public class LIB
	{
		private LIB ()
		{
		}

		public static String extractFileName(String fileName)
    	{
    		return (fileName.LastIndexOf('/')==0 ? fileName : fileName.Substring(fileName.LastIndexOf('/')+1));
    	}

		public static String readTextTCP (NetworkStream io)
		{
	        String line = "";
	        char ch;
	        
	        while((ch = (char)io.ReadByte()) != 0)
	        	line += ch;

	        return line;
		}

		public static void writeTextTCP(NetworkStream outToServer, String line)
		{
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
			outToServer.Write(encoding.GetBytes(line), 0, line.Length);
			outToServer.WriteByte(0);
		}

	    public static long getFileSizeTCP(NetworkStream inFromServer)
	    {
	    	return long.Parse(readTextTCP(inFromServer));
	    }

		public static long check_File_Exists (String fileName)
		{
			if (File.Exists (fileName))
				return (new FileInfo(fileName)).Length;

			return 0;
		}
	}
}

