// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a content-format option specified by RFC 7252. It indicates the representation format of the
    /// message payload.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.UIntOption" />
    public abstract class ContentFormat : UIntOption
    {
        private const ushort NUMBER = 12;
        private const ushort MAX_LENGTH = 2;
        private const ushort MIN_LENGTH = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFormat"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="ContentFormat"/>.</param>
        /// <param name="mediaType">The media type string of that <see cref="ContentFormat"/>.</param>
        /// <param name="encodings">The set of encodings that are supported by this <see cref="ContentFormat"/>.</param>
        protected ContentFormat(uint id, string mediaType, IEnumerable<Encoding> encodings)
            : base(NUMBER, id, MIN_LENGTH, MAX_LENGTH, false)
        {
            this.MediaType = mediaType;
            this.Encodings = new List<Encoding>(encodings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFormat"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="ContentFormat"/>.</param>
        /// <param name="mediaType">The media type string of that <see cref="ContentFormat"/>.</param>
        protected ContentFormat(uint id, string mediaType)
            : this(id, mediaType, Enumerable.Empty<Encoding>())
        {
        }

        /// <summary>
        /// Gets the media type string of that <see cref="ContentFormat"/>.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string MediaType { get; }

        /// <summary>
        /// Gets the set of encodings that are supported by that <see cref="ContentFormat"/>.
        /// </summary>
        /// <value>
        /// The encodings.
        /// </value>
        public IReadOnlyList<Encoding> Encodings { get; }

        /// <summary>
        /// Gets the identifier of that <see cref="ContentFormat"/>.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public uint Id => this.Value;

        /// <inheritdoc />
        public override string ToString() => $"{this.Name} ({this.Number}): {this.MediaType}";

        /// <summary>
        /// Provides functionality to create a <see cref="ContentFormat"/> from a specified <see cref="OptionData"/>.
        /// </summary>
        /// <seealso cref="WorldDirect.CoAP.IOptionFactory" />
        public class Factory : IOptionFactory
        {
            private readonly ContentFormatRegistry registry;

            /// <summary>
            /// Initializes a new instance of the <see cref="Factory"/> class.
            /// </summary>
            /// <param name="registry">The <see cref="ContentFormatRegistry"/> that holds all available <see cref="ContentFormat"/>s.</param>
            public Factory(ContentFormatRegistry registry)
            {
                this.registry = registry;
            }

            /// <inheritdoc />
            public int Number => NUMBER;

            /// <inheritdoc />
            /// <exception cref="ArgumentOutOfRangeException">Throws if the number of the read <see cref="OptionData"/> does not match the factory's number.</exception>
            /// <exception cref="InvalidOperationException">Throws if the <see cref="ContentFormat"/> with the read id could not be found.</exception>
            public CoapOption Create(OptionData src)
            {
                if (src.Number != NUMBER)
                {
                    throw new ArgumentOutOfRangeException(nameof(src), src.Number, "Option data number is not valid for Content-Format factory.");
                }

                var id = MemoryReader.ReadUInt32BigEndian(src.Value);
                var contentFormat = this.registry.SingleOrDefault(c => c.Id.Equals(id));
                if (contentFormat == null)
                {
                    throw new InvalidOperationException($"Can not find Content-Format option with id {id}.");
                }

                return contentFormat;
            }
        }
    }
}
