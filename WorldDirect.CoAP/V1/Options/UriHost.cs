// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriHost : StringOption
    {
        public const ushort NUMBER = 3;
        private const ushort MAX_LENGTH = 255;
        private const ushort MIN_LENGTH = 1;

        public UriHost(string value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public UriHost(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => NUMBER;

            public CoapOption Create(OptionData src)
            {
                if (src.Number != this.Number)
                {
                    throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Host factory.");
                }

                return new UriHost(src.Value);
            }
        }
    }
}
