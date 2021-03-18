// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Common.Extensions;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as in <see cref="uint"/> format. This means the value
    /// of the <see cref="CoapOption"/> is a <see cref="uint"/>.
    /// </summary>
    public abstract class UIntOptionFormat : CoapOption<uint>
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that option.</param>
        /// <remarks>
        /// If the system's computer architecture is in little endian order, the <paramref name="value"/> will be
        /// reversed, because the <paramref name="value"/> is in network byte order (big endian order).
        /// </remarks>
        protected UIntOptionFormat(ushort number, uint value, uint maxLength, uint minLength)
            : base(number, value, maxLength, minLength, Constructor)
        {
        }

        protected UIntOptionFormat(ushort number, uint value, uint maxLength)
            : this(number, value, maxLength, MIN_LENGTH)
        {
        }

        protected UIntOptionFormat(ushort number, byte[] value, uint maxLength)
            : this(number, value, maxLength, MIN_LENGTH)
        {
        }

        protected UIntOptionFormat(ushort number, byte[] value, uint maxLength, uint minLength)
            : base(number, value, maxLength, minLength, bytes => BinaryPrimitives.ReadUInt32BigEndian(bytes))
        {
        }

        private static byte[] Constructor(uint value)
        {
            var buffer = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            return buffer.RemoveLeadingZeros();
        }
    }
}
