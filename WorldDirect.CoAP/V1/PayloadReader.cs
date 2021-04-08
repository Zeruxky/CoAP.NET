namespace WorldDirect.CoAP.V1
{
    using System;
    using Common.Extensions;
    using Options;

    /// <summary>
    /// Provides functionality to read the payload of the message from the specified <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.IReader{System.ReadOnlyMemory{System.Byte}}" />
    public class PayloadReader : IReader<ReadOnlyMemory<byte>>
    {
        private static readonly byte[] EmptyPayload = new byte[0];
        private const byte PAYLOAD_MARKER = 0xFF;

        /// <inheritdoc />
        public int Read(ReadOnlyMemory<byte> value, out ReadOnlyMemory<byte> result)
        {
            if (value.IsEmpty)
            {
                result = EmptyPayload;
                return 0;
            }

            if (value.Span[0] != PAYLOAD_MARKER || value.Length == 1)
            {
                throw new MessageFormatErrorException("Payload marker found but no payload.");
            }

            result = MemoryReader.ReadBytesBigEndian(value, 1);
            return value.Length;
        }
    }
}
