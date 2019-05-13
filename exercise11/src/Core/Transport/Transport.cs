using System;

namespace Core
{
    public class Transport : ITransport
    {
        public int MaxSize { get; private set; }
        public ISLIP Slip { get; private set; }
        public ITimeout Timeout { get; private set; }
        private int _retries = 0;

        public Transport(string device, int maxSize)
        {
            this.MaxSize = maxSize;
            this.Slip = new SLIP(device);
            this.Timeout = new Timeout();
        }

        public void Send(byte[] msg)
        {
            int l = msg.Length;
            int ite = 0; // Sequence
            _retries = 0;
            byte[] current = new byte[this.MaxSize];

            // Stop-and-wait - send message in chunks
            do
            {
                // Get sub array of (maximum) size
                current = msg.SubArray(this.MaxSize * ite, this.MaxSize * (ite + 1) - 1); // 0-999, 1000-1999, 2000-2999 etc.
                
                // Send chunk and handle stop-and-wait
                HandleChunk(current, ite);

                ite++;
                l -= this.MaxSize;
            }
            while (l > 0);
        }

        public int Receive(ref byte[] buffer)
        {
            return this.Slip.Receive(ref buffer, buffer.Length);
        }

        private void HandleChunk(byte[] msg, int seqNo)
        {
            // Send chunk
            SendChunk(msg, seqNo);

            // Wait for ACK
            int res = WaitForAck(seqNo);

            // Resend if wrong ACK
            if (res != seqNo)
            {
                if (_retries == 5)
                {
                    // TODO: Handle maximum retries
                    return;
                }
                else
                {
                    _retries++;
                    HandleChunk(msg, seqNo);
                }
            }
            else if (res == -1)
            {
                // TODO: Handle NOT ACK received
            }
            // Else correct ack received
        }

        private void SendChunk(byte[] msg, int seqNo)
        {
            var chksum = new Checksum(msg);
            byte chksumHigh = (byte) ((chksum.Value >> 8) & 0xFF);
            byte chksumLow = (byte) (chksum.Value & 0xFF);

            byte seq = (byte) seqNo;
            var type = Core.Type.DATA;

            var header = new Header(chksumHigh, chksumLow, seq, type);
            var data = new Data(msg);

            var packet = (byte[]) (new Packet(header, data));

            this.Slip.Send(packet, packet.Length);
        }

        private void SendAck(int seqNo)
        {
            var ackHeader = new Header((byte) seqNo, Core.Type.ACK);
            var packet = (byte[]) (new Packet(ackHeader, null));

            this.Slip.Send(packet, packet.Length);
        }

        private int WaitForAck(int seq)
        {
            byte[] buf = new byte[this.MaxSize * 2]; // ACK packet should never be bigger than this

            int recv = this.Slip.Receive(ref buf, buf.Length);

            if (buf[3] != (byte) Core.Type.ACK) return -1;

            if (buf[2] != seq) return buf[2];

            return seq;
        }
    }
}