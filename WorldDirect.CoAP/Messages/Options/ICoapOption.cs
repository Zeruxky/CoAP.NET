namespace WorldDirect.CoAP.Messages.Options
{
    public interface ICoapOption
    {
        ushort Number { get; }

        byte[] RawValue { get; }
    }
}
