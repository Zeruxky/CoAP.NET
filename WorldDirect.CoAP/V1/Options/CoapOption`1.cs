// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Text;
    using Common.Extensions;

    public abstract class CoapOption<T> : CoapOption
    {
        protected CoapOption(ushort number, byte[] value, uint maxLength, uint minLength, Func<byte[], T> constructor)
            : base(number, value.RemoveLeadingZeros(), maxLength, minLength)
        {
            this.Value = constructor(value);
        }

        protected CoapOption(ushort number, byte[] value, uint maxLength, Func<byte[], T> constructor)
            : this(number, value, maxLength, 0, constructor)
        {
        }

        protected CoapOption(ushort number, T value, uint maxLength, uint minLength, Func<T, byte[]> constructor)
            : base(number, constructor(value).RemoveLeadingZeros(), maxLength, minLength)
        {
            this.Value = value;
        }

        protected CoapOption(ushort number, T value, uint maxLength, Func<T, byte[]> constructor)
            : this(number, value, maxLength, 0, constructor)
        {
        }

        public new T Value { get; }

        public override string ToString() => $"{base.ToString()}: {this.Value}";
    }
}
