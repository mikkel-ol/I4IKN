using System;

namespace Core
{
    public class Transport : ITransport
    {
        public int MaxSize { get; private set; }
        public ISLIP Slip { get; private set; }
        public ITimeout Timeout { get; private set; }
        private int _retries = 0;
        private bool _sequence = false; // Alternate between 0 and 1

        public Transport(string device, int maxSize)
        {
            this.MaxSize = maxSize;
            this.Slip = new SLIP(device);
            this.Timeout = new Timeout();
        }

        public void Send(byte[] data)
        {
            int l = data.Length;
            int ite = 0; // Getting the right subarray
            _retries = 0;
            byte[] current = new byte[this.MaxSize];

            // Stop-and-wait - send message in chunks
            do
            {
                // TODO: Handle maximum number of retries
                if (_retries > 5) break;

                // Get sub array of (maximum) size
                int cl = (this.MaxSize * (ite + 1) - 1) < l ? this.MaxSize * (ite + 1) - 1 : l;
                current = data.SubArray(this.MaxSize * ite, cl); // 0-999, 1000-1999, 2000-2999 etc.

                // Pack message and send it
                var packet = Pack(current, _sequence);
                this.Slip.Send(packet, packet.Length);

                // Wait for ACK
                int res = WaitForAck();

                // If ACK is not the same _sequence number as current, send pack again (loop again)
                if (res != Convert.ToInt32(_sequence))
                {
                    _retries++;
                }
                // Else count up and continue
                else
                {
                    ite++;
                    _sequence = !_sequence;
                    l -= this.MaxSize;
                }
            }
            while (l > 0);
        }

        // TODO: Handle already received packets
        public void Receive(ref byte[] buffer)
        {
            var buf = new byte[this.MaxSize];
            int res = 0;
            int ite = 1;

            do
            {
                res = this.Slip.Receive(ref buf, buf.Length);
                if (res == -1) throw new ReceiveException("Error while receiving");
                // TODO: Might need to handle res == 0 here (null errors below..)

                var pack = (Packet) buf;
                var calcChecksum = new Checksum(pack.Data);
                var recvChecksum = new Checksum(pack.Header.CS_HI, pack.Header.CS_LO);
                var seq = Convert.ToBoolean(pack.Header.SEQ);

                // TODO: Might need to handle differently
                if (calcChecksum != recvChecksum)
                    SendAck(!seq); // Send ACK of previous
                
                else SendAck(seq);

                // Copy current data in packet to buffer
                var data = (byte[]) pack.Data;
                Buffer.BlockCopy(data, 0, buffer, ite * this.MaxSize, data.Length);

                ite++;
            } 
            while (res != 0);
        }

        private byte[] Pack(byte[] msg, bool seqNo)
        {
            var chksum = new Checksum(msg);
            byte chksumHigh = (byte) ((chksum.Value >> 8) & 0xFF);
            byte chksumLow = (byte) (chksum.Value & 0xFF);

            byte seq = (byte) Convert.ToInt32(seqNo);
            var type = Core.Type.DATA;

            var header = new Header(chksumHigh, chksumLow, seq, type);
            var data = new Data(msg);

            return (byte[]) (new Packet(header, data));
        }

        private void SendAck(bool seqNo)
        {
            var ackHeader = new Header((byte) Convert.ToInt32(seqNo), Core.Type.ACK);
            var packet = (byte[]) (new Packet(ackHeader, null));

            this.Slip.Send(packet, packet.Length);
        }

        private int WaitForAck()
        {
            byte[] buf = new byte[this.MaxSize * 2]; // ACK packet should never be bigger than this

            int recv = this.Slip.Receive(ref buf, buf.Length);

            if (buf[3] != (byte)Core.Type.ACK) return -1;

            return buf[2];
        }
    }
}