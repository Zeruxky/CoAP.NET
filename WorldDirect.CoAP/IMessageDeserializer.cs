// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using WorldDirect.CoAP.Messages;

    /// <summary>
    /// Defines the functionality to deserialize a <see cref="CoapMessage"/> from a given <see cref="Memory{T}"/> of <see cref="byte"/>s.
    /// </summary>
    public interface IMessageDeserializer
    {
        /// <summary>
        /// Deserializes the given <paramref name="value"/> to a <see cref="CoapMessage"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> of <see cref="byte"/>s for deserialization.</param>
        /// <returns>A <see cref="CoapMessage"/> that is equivalent to the given <paramref name="value"/>.</returns>
        CoapMessage Deserialize(ReadOnlySpan<byte> value);

        bool CanDeserialize(CoapVersion version);
    }
}
