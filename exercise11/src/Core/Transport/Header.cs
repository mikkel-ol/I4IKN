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
    }
}