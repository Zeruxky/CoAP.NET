﻿// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Codes.MethodCodes;
    using WorldDirect.CoAP.V1.Options;

    public class CoapMessage : IEquatable<CoapMessage>
    {
        public CoapMessage(CoapHeader header)
            : this(header, CoapToken.EmptyToken, new List<CoapOption>(), ReadOnlyMemory<byte>.Empty)
        {
        }

        public CoapMessage(CoapHeader header, CoapToken token, IEnumerable<CoapOption> options, ReadOnlyMemory<byte> payload)
        {
            this.Header = header;
            this.Token = token;
            this.Option = new ReadOnlyOptionCollection(options);
            this.Payload = payload;
        }

        public CoapHeader Header { get; }

        public CoapToken Token { get; }

        public ReadOnlyOptionCollection Option { get; }

        public ReadOnlyMemory<byte> Payload { get; }

        public bool IsEmptyMessage
        {
            get
            {
                if (!this.Header.Code.Equals(CoapCodes.Empty))
                {
                    return false;
                }

                if (!this.Payload.IsEmpty)
                {
                    // Empty message contains payload -> invalid.
                    throw new MessageFormatErrorException();
                }

                return true;
            }
        }

        public bool Equals(CoapMessage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Header.Equals(other.Header) && this.Token.Equals(other.Token) && this.Option.SequenceEqual(other.Option) && this.Payload.Span.SequenceEqual(other.Payload.Span);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CoapMessage) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Header, this.Token, this.Option, this.Payload);
        }

        public static bool operator ==(CoapMessage left, CoapMessage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapMessage left, CoapMessage right)
        {
            return !Equals(left, right);
        }
    }
}
