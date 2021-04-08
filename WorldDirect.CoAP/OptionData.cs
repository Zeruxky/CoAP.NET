// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;

    public ref struct OptionData
    {
        public OptionData(ushort offset, ushort delta, ushort length, ReadOnlyMemory<byte> value)
        {
            this.Number = (ushort)(offset + delta);
            this.Length = length;
            this.Value = value;
        }

        public ushort Number { get; }

        public ushort Length { get; }

        /// <summary>
        /// Gets the value of the option.
        /// </summary>
        /// <value>
        /// The read value of the option.
        /// </value>
        public ReadOnlyMemory<byte> Value { get; }
    }
}
