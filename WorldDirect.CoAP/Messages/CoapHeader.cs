// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Messages
{
    using System;
    using WorldDirect.CoAP.Messages.Codes;

    /// <summary>
    /// Represents the header of a <see cref="CoapMessage"/>.
    /// </summary>
    /// <seealso cref="System.IEquatable{CoapHeader}" />
    public readonly struct CoapHeader : IEquatable<CoapHeader>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoapHeader"/> struct.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="type">The type.</param>
        public CoapHeader(CoapVersion version, CoapType type)
            : this(version, type, CoapTokenLength.Default, new EmptyCode(), CoapMessageId.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapHeader"/> struct.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="type">The type.</param>
        /// <param name="length">The length.</param>
        /// <param name="code">The code.</param>
        /// <param name="id">The identifier.</param>
        public CoapHeader(CoapVersion version, CoapType type, CoapTokenLength length, CoapCode code, CoapMessageId id)
        {
            this.Version = version;
            this.Type = type;
            this.Length = length;
            this.Code = code;
            this.Id = id;
        }

        /// <summary>
        /// Gets the <see cref="CoapVersion"/> of that <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public CoapVersion Version { get; }

        /// <summary>
        /// Gets the <see cref="CoapType"/> of that <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public CoapType Type { get; }

        /// <summary>
        /// Gets the <see cref="CoapTokenLength"/> of that <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public CoapTokenLength Length { get; }

        /// <summary>
        /// Gets the <see cref="ICoapCode"/> of that <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public CoapCode Code { get; }

        public CoapMessageId Id { get; }

        public static bool operator ==(CoapHeader left, CoapHeader right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapHeader left, CoapHeader right)
        {
            return !Equals(left, right);
        }

        public bool Equals(CoapHeader other)
        {
            return this.Version.Equals(other.Version) &&
                   this.Type.Equals(other.Type) &&
                   this.Length.Equals(other.Length) &&
                   this.Code.Equals(other.Code) &&
                   this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CoapHeader)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Version, this.Type, this.Length, this.Code, this.Id);
        }
    }
}
