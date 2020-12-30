namespace WorldDirect.CoAP
{
    public interface ISerializer<in TIn>
    {
        bool CanSerialize(Version version);

        byte[] Serialize(TIn value);
    }
}