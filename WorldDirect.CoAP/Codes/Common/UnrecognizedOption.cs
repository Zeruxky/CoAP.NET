// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Codes.Common
{
    using System;
    using CoAP.Common.Extensions;
    using WorldDirect.CoAP.V1.Options;

    public class UnrecognizedOption : CoapOption<ReadOnlyMemory<byte>>
    {
        public UnrecognizedOption(ushort number, ReadOnlyMemory<byte> rawValue)
            : base(number, rawValue, uint.MaxValue, true)
        {
        }

        public override string ToString() => $"{base.ToString()}: {this.Value.Span.ToString(' ')}";
    }
}
