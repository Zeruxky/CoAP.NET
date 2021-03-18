// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Linq;
    using Common.Extensions;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as a opaque sequence of bytes. This means the value of that <see cref="CoapOption"/>
    /// is a opaque sequence of bytes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.OpaqueOptionFormat}" />
    public abstract class OpaqueOptionFormat : CoapOption<byte[]>
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpaqueOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value" /> will be
        /// reversed, because the <paramref name="value" /> is in network byte order (big endian order).
        /// </remarks>
        protected OpaqueOptionFormat(ushort number, byte[] value, uint maxLength, uint minLength)
            : base(number, value, maxLength, minLength, Constructor)
        {
        }

        protected OpaqueOptionFormat(ushort number, byte[] value, uint maxlength)
            : this(number, value, maxlength, MIN_LENGTH)
        {
        }

        public override string ToString() => $"{base.ToString()}: {this.Value.ToString(' ')}";

        private static byte[] Constructor(byte[] value)
        {
            var buffer = BitConverter.IsLittleEndian
                ? value.Reverse().ToArray()
                : value;

            return buffer.RemoveLeadingZeros();
        }
    }
}
