// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.V1.Messages;
    using WorldDirect.CoAP.V1.Options;

    /// <summary>
    /// Provides functionality to serialize and deserialize a <see cref="CoapMessage"/> that is compliant to RFC 7252.
    /// </summary>
    public sealed class CoapMessageSerializer : IMessageSerializer
    {
        private readonly IReader<IReadOnlyCollection<CoapOption>> optionReader;
        private readonly IReader<ReadOnlyMemory<byte>> payloadReader;
        private readonly IReader<CoapHeader> headerReader;
        private readonly IReader<CoapToken> tokenReader;

        public CoapMessageSerializer(
            IReader<CoapHeader> headerReader,
            IReader<CoapToken> tokenReader,
            IReader<IReadOnlyCollection<CoapOption>> optionsReader,
            IReader<ReadOnlyMemory<byte>> payloadReader)
        {
            this.headerReader = headerReader;
            this.tokenReader = tokenReader;
            this.optionReader = optionsReader;
            this.payloadReader = payloadReader;
        }

        public CoapMessage Deserialize(ReadOnlyMemory<byte> value)
        {
            var position = this.headerReader.Read(value.Slice(0, 4), out var header);
            position += this.tokenReader.Read(value.Slice(position, (UInt4)header.Length), out var token);
            position += this.optionReader.Read(value.Slice(position), out var options);
            position += this.payloadReader.Read(value.Slice(position), out var payload);
            return new CoapMessage(header, token, options, payload);
        }

        public bool CanDeserialize(UdpReceiveResult result)
        {
            this.headerReader.Read(result.Buffer, out var header);
            return header.Version.Equals(CoapVersion.V1);
        }
    }
}
