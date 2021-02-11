// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.Common
{
    using System;
    using WorldDirect.CoAP.V1.Options;

    public class UnknownOption : CoapOption
    {
        public UnknownOption(ushort number, byte[] rawValue)
            : base(rawValue)
        {
            this.Number = number;
        }

        public override ushort Number { get; }

        public override string ToString() => $"Unknown Option ({this.Number}): {BitConverter.ToString(this.RawValue).Replace('-', ' ')}";
    }
}
