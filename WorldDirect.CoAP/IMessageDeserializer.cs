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
        /// Inserts the given <paramref name="value"/> in the pipeline for deserialization to a <see cref="CoapMessage"/>.
        /// </summary>
        /// <param name="value">The <see cref="Memory{T}"/> of <see cref="byte"/>s for deserialization.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(Memory<byte> value, CancellationToken ct = default);

        /// <summary>
        /// Gets the next available <see cref="CoapMessage"/> from the deserialization pipeline.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous get operation. The result is a
        /// <see cref="CoapMessage"/>.</returns>
        Task<CoapMessage> GetAsync(CancellationToken ct = default);
    }
}
