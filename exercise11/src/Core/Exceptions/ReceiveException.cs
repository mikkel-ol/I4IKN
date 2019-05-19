[System.Serializable]
public class ReceiveException : System.Exception
{
    public ReceiveException() { }
    public ReceiveException(string message) : base(message) { }
    public ReceiveException(string message, System.Exception inner) : base(message, inner) { }
    protected ReceiveException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}