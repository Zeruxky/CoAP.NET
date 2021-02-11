namespace WorldDirect.CoAP.V1
{
    using System;

    public class PayloadReader : IReader<ReadOnlyMemory<byte>>
    {
        private const byte PAYLOAD_MARKER = 0xFF;

        /// <inheritdoc />
        public int Read(ReadOnlyMemory<byte> value, out ReadOnlyMemory<byte> result)
        {
            if (value.IsEmpty)
            {
                result = new byte[0];
                return 0;
            }

            if (value.Span[0] != PAYLOAD_MARKER || value.Length == 1)
            {
                throw new MessageFormatErrorException("Payload marker found but no payload.");
            }

            result = value.Slice(1);
            return value.Length;
        }
    }
}