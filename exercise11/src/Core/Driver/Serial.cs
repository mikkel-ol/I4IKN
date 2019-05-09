using System;
using System.IO.Ports;

namespace Core
{
    public class Serial : ISerial
    {
        private SerialPort _port;

        public Serial(
            string path = "/dev/ttyS0",
            int baud = 115200,
            Parity par = Parity.None,
            int databits = 8,
            StopBits stop = StopBits.One)
        {
            _port = new SerialPort(path, baud, (System.IO.Ports.Parity) par, databits, (System.IO.Ports.StopBits) stop);
        }

        ~Serial()
        {
            _port.Close();
        }

        public bool Open()
        {
            try
            {
                _port.Open();
            }
            catch (UnauthorizedAccessException)
            {
                // Port is in use or access denied
                return false;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return true;
        }

        public bool Send(byte[] data, int length)
        {
            try
            {
                _port.Write(data, 0, length);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public int Receive(ref byte[] buffer, int length = 4096)
        {
            try
            {
                return _port.Read(buffer, 0, length);
            }
            catch (Exception)
            {
                return -1;
            }
        }        
    }
}