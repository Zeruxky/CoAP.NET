// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Buffers.Binary;
    using System.Linq;
    using System.Text;
    using Common.Extensions;

    public ref struct OptionData
    {
        public OptionData(ushort offset, ushort delta, ushort length, ReadOnlySpan<byte> value)
        {
            this.Number = (ushort)(offset + delta);
            this.Length = length;
            this.Value = BitConverter.IsLittleEndian ? value.Reverse().ToArray() : value.ToArray();
            this.UIntValue = BinaryPrimitives.ReadUInt32BigEndian(value.AlignByteArray(4));
            this.StringValue = Encoding.UTF8.GetString(this.Value);
        }

        public ushort Number { get; }

        public ushort Length { get; }

        public byte[] Value { get; }

        public string StringValue { get; }

        public uint UIntValue { get; }
    }
}
