﻿// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a <see cref="CoapOption"/> in <see cref="string"/> format. This means the value of that <see cref="CoapOption"/>
    /// represents a unicode <see cref="string"/> that is encoded using UTF-8 in Net-Unicode form.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Messages.Options.CoapOption" />
    /// <seealso cref="System.IEquatable{WorldDirect.CoAP.Messages.Options.StringOptionFormat}" />
    public abstract class StringOptionFormat : CoapOption
    {
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringOptionFormat"/> class.
        /// </summary>
        /// <param name="value">The byte array (in network byte order) that represents the value of that <see cref="CoapOption"/>.</param>
        /// <remarks>
        /// To be compliant with the RFC 7252, we encoding the byte array by using UTF-8 and
        /// normalize the string by using the Unicode normalization form "NFC".
        /// </remarks>
        protected StringOptionFormat(ushort number, string value, uint maxLength, uint minLength)
            : base(number, Encoding.UTF8.GetBytes(value), maxLength, minLength)
        {
            this.Value = value;
        }

        protected StringOptionFormat(ushort number, string value, uint maxLength)
            : this(number, value, maxLength, MIN_LENGTH)
        {
        }

        protected StringOptionFormat(ushort number, byte[] value, uint maxLength, uint minLength)
            : base(number, value, maxLength, minLength)
        {
            this.Value = Encoding.UTF8.GetString(value);
        }

        protected StringOptionFormat(ushort number, byte[] value, uint maxLength)
            : this(number, value, maxLength, MIN_LENGTH)
        {
        }

        public string Value { get; }

        public override string ToString() => $"{base.ToString()}: {this.Value}";
    }
}
