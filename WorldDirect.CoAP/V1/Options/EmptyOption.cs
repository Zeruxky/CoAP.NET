// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> as empty. This means the value of this <see cref="CoapOption"/>
    /// is a zero-length sequence of bytes.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.EmptyOptionFormat}" />
    public abstract class EmptyOption : CoapOption<Memory<byte>>
    {
        private const ushort MAX_LENGTH = 0;
        private static readonly Memory<byte> EmptyByteArray = Memory<byte>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyOption"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="EmptyOption"/>.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="EmptyOption"/> can appear multiple times
        /// in an <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected EmptyOption(ushort number, bool isRepeatable)
            : base(number, EmptyByteArray, MAX_LENGTH, isRepeatable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyOption"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="EmptyOption"/>.</param>
        protected EmptyOption(ushort number)
            : this(number, false)
        {
        }
    }
}
