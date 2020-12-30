namespace WorldDirect.CoAP
{
    using System;

    public interface IOption
    {
        ushort Number { get; }

        byte[] RawValue { get; set; }
    }
}
