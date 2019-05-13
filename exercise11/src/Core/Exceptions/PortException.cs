[System.Serializable]
public class PortException : System.Exception
{
    public PortException() { }
    public PortException(string message) : base(message) { }
    public PortException(string message, System.Exception inner) : base(message, inner) { }
    protected PortException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}