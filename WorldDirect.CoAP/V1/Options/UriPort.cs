// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class UriPort : UIntOption
    {
        public const ushort NUMBER = 7;
        private const ushort MAX_LENGTH = 2;
        private const ushort MIN_LENGTH = 0;

        public UriPort(uint value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public UriPort(ReadOnlyMemory<byte> value)
            : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
        {
        }

        public class Factory : IOptionFactory
        {
            public int Number => UriPort.NUMBER;

            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentException($"Option data number {src.Number} is not valid for Uri-Port factory.");
                }

                return new UriPort(src.Value);
            }
        }
    }
}
