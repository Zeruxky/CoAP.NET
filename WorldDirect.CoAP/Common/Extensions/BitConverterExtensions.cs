// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Common.Extensions
{
    using System.IO;

    public static class BitConverterExtensions
    {
        public static ushort ReadUInt16AsBigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            return bytes.ToUInt16AsBigEndian();
        }

        public static uint ReadUInt32AsBigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return bytes.ToUInt32AsBigEndian();
        }

        public static ulong ReadUInt64AsBigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            return bytes.ToUInt64AsBigEndian();
        }
    }
}