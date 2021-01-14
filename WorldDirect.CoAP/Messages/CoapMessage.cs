// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Messages.Options;

    public class CoapMessage : IEquatable<CoapMessage>
    {
        public CoapMessage(CoapHeader header)
            : this(header, CoapToken.EmptyToken, new List<ICoapOption>(), CoapPayload.EmptyPayload)
        {
        }

        public CoapMessage(CoapHeader header, CoapToken token, IEnumerable<ICoapOption> options, CoapPayload payload)
        {
            this.Header = header;
            this.Token = token;
            this.Options = new List<ICoapOption>(options.OrderBy(o => o.Number));
            this.Payload = payload;
        }

        public CoapHeader Header { get; }

        public CoapToken Token { get; }

        public IReadOnlyList<ICoapOption> Options { get; }

        public CoapPayload Payload { get; }

        public static bool operator ==(CoapMessage left, CoapMessage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapMessage left, CoapMessage right)
        {
            return !Equals(left, right);
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

            return this.Header.Equals(other.Header) &&
                   this.Token.Equals(other.Token) &&
                   this.Options.SequenceEqual(other.Options) &&
                   this.Payload.Equals(other.Payload);
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

            return this.Equals((CoapMessage)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Header, this.Token, this.Options, this.Payload);
        }
    }

    public class CoapRequestMessage
    {
        public 
    }
}
