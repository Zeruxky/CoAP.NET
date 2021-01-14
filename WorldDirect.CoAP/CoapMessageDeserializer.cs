// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using WorldDirect.CoAP.Messages;
    using WorldDirect.CoAP.Messages.Codes;
    using WorldDirect.CoAP.Messages.Options;

    /// <summary>
    /// Defines the core behaviour of a deserialization a <see cref="CoapMessage"/> from a <see cref="Memory{T}"/> of <see cref="byte"/>s.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.IMessageDeserializer" />
    public class CoapMessageDeserializer : IMessageDeserializer, IDisposable
    {
        private bool disposed = false;

        /// <summary>
        /// The <see cref="CodeRegistry"/> that is used for getting all known <see cref="ICoapCode"/>s.
        /// </summary>
        protected readonly CodeRegistry Registry;

        /// <summary>
        /// The <see cref="OptionsFactory"/> that is used for constructing a <see cref="ICoapOption"/> by a given number.
        /// </summary>
        protected readonly OptionsFactory Factory;

        /// <summary>
        /// The <see cref="BufferBlock{T}"/> that stores all incoming <see cref="Memory{T}"/>s of type <see cref="byte"/>.
        /// </summary>
        protected BufferBlock<Memory<byte>> InputBlock;

        /// <summary>
        /// The <see cref="BufferBlock{T}"/> that stores all deserialized <see cref="CoapMessage"/>s.
        /// </summary>
        protected BufferBlock<IDeserializerResult<CoapMessage>> OutputBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessageDeserializer"/> class.
        /// </summary>
        /// <param name="registry">The <see cref="CodeRegistry"/> that should be used by the <see cref="CoapMessageDeserializer"/>.</param>
        /// <param name="factory">The <see cref="OptionsFactory"/> that should be used by the <see cref="CoapMessageDeserializer"/>.</param>
        protected CoapMessageDeserializer(CodeRegistry registry, OptionsFactory factory)
        {
            this.Registry = registry;
            this.Factory = factory;
            this.InputBlock = new BufferBlock<Memory<byte>>();
            this.OutputBlock = new BufferBlock<IDeserializerResult<CoapMessage>>();
        }

        /// <inheritdoc />
        public virtual Task InsertAsync(Memory<byte> value, CancellationToken ct = default)
        {
            return this.InputBlock.SendAsync(value, ct);
        }

        /// <inheritdoc />
        public virtual async Task<CoapMessage> GetAsync(CancellationToken ct = default)
        {
            var result = await this.OutputBlock.ReceiveAsync(ct).ConfigureAwait(false);
            if (result.Failed)
            {
                throw result.CausingException;
            }

            return result.Value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }
    }
}
