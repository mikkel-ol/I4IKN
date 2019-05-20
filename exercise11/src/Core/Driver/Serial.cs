using System;
using System.IO.Ports;

namespace Core
{
    public class Serial : ISerial
    {
        public SerialPort Port { get; private set; }

        public Serial(
            string path = "/dev/ttyS0",
            int baud = 115200,
            Parity par = Parity.None,
            int databits = 8,
            StopBits stop = StopBits.One)
        {
            Port = new SerialPort(path, baud, (System.IO.Ports.Parity) par, databits, (System.IO.Ports.StopBits) stop);
            Open();
        }

        ~Serial()
        {
            Port.Close();
        }

        public void Open()
        {
            try
            {
                Port.Open();
            }
            catch (UnauthorizedAccessException)
            {
                // Port is in use or access denied
                throw new PortException("Access denied to serial port or port already opened");
            }
            catch (ArgumentException)
            {
                throw new PortException("Port name invalid");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public bool Send(byte[] data, int length)
        {
            try
            {
                Port.Write(data, 0, length);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return false;
            }
            return true;
        }

        public int Receive(ref byte[] buffer, int length = 4096)
        {
            try
            {
                return Port.Read(buffer, 0, length);
            }
            catch (Exception)
            {
                return -1;
            }
        }        
    }
}