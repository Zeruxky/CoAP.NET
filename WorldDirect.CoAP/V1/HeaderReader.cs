// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Buffers.Binary;
    using System.Linq;
    using WorldDirect.CoAP.Codes;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.V1.Messages;

    /// <summary>
    /// Provides functionality to read a <see cref="CoapHeader"/> from a given <see cref="Memory{T}"/> value,
    /// that is compliant to the RFC 7252.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.IReader{WorldDirect.CoAP.V1.Messages.CoapHeader}" />
    public class HeaderReader : IReader<CoapHeader>
    {
        private const byte MASK_VERSION = 0xC0;
        private const byte MASK_TYPE = 0x30;
        private const byte MASK_TOKEN_LENGTH = 0x0F;
        private const byte MASK_CODE_CLASS = 0xE0;
        private const byte MASK_CODE_DETAIL = 0x1F;
        private readonly CodeRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderReader"/> class.
        /// </summary>
        /// <param name="registry">The <see cref="CodeRegistry"/> that holds all <see cref="CoapCode"/>s that are used in this application.</param>
        public HeaderReader(CodeRegistry registry)
        {
            this.registry = registry;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Throws, if the version of that read header is not 1.</exception>
        /// <exception cref="MessageFormatErrorException">Throws, if the token length is greater than 8.</exception>
        /// <exception cref="InvalidOperationException">The read <see cref="CoapCode"/> is a unknown code.</exception>
        public int Read(ReadOnlyMemory<byte> value, out CoapHeader result)
        {
            var version = (CoapVersion)(UInt2)((value.Span[0] & MASK_VERSION) >> 6);
            if (!HeaderReader.CanRead(version))
            {
                throw new ArgumentException("Can only read headers for Version 1.", nameof(version));
            }

            var type = (CoapType)(UInt2)((value.Span[0] & MASK_TYPE) >> 4);
            var tokenLength = (CoapTokenLength)(UInt4)(value.Span[0] & MASK_TOKEN_LENGTH);
            if ((UInt4)tokenLength > 8)
            {
                throw new MessageFormatErrorException("Token lengths of 9 to 15 are reserved.");
            }

            var codeClass = (CodeClass)(UInt3)((value.Span[1] & MASK_CODE_CLASS) >> 5);
            var codeDetail = (CodeDetail)(UInt5)(value.Span[1] & MASK_CODE_DETAIL);
            var key = new CoapCodeKey(codeClass, codeDetail);
            var code = this.registry.SingleOrDefault(c => c.Key.Equals(key));
            if (code == null)
            {
                throw new InvalidOperationException($"Can not find code with key {key}.");
            }

            var messageId = (CoapMessageId)BinaryPrimitives.ReadUInt16BigEndian(value.Span.Slice(2, 2));
            result = new CoapHeader(version, type, tokenLength, code, messageId);
            return 4;
        }

        private static bool CanRead(CoapVersion version)
        {
            return version.Equals(CoapVersion.V1);
        }
    }
}
