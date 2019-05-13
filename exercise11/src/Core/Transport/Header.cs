namespace Core
{
    public class Header : IHeader
    {
        public Header(byte csh, byte csl, byte seq, Type type)
        {
            CS_HI = csh;
            CS_LO = csl;
            SEQ = seq;
            TYPE = (byte) type;
        }

        public Header(byte seq, Type type)
        {
            if (type != Core.Type.ACK) throw new HeaderException("Type must be ACK to use Header constructor with no checksum");

            CS_HI = 0;
            CS_LO = 0;
            SEQ = seq;
            TYPE = (byte) Core.Type.ACK;
        }
    }
}