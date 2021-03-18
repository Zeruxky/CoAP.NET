// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.Common
{
    using System;
    using CoAP.Common.Extensions;
    using WorldDirect.CoAP.V1.Options;

    public class UnknownOption : CoapOption<byte[]>
    {
        public UnknownOption(ushort number, byte[] rawValue)
            : base(number, rawValue, uint.MaxValue, Constructor)
        {
        }

        private static byte[] Constructor(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(value);
            }

            return value.RemoveLeadingZeros();
        }
    }
}
