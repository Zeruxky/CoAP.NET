namespace WorldDirect.CoAP
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public struct MessageId : IEquatable<MessageId>
    {
        /// <summary>
        /// The default value of a <see cref="MessageId"/>. It represents a <see cref="MessageId"/> with value 0.
        /// </summary>
        public static readonly MessageId Default = new MessageId(0);

        private static readonly Random Random = new Random();

        // This mutex protects the access to the Random field.
        private static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        private readonly ushort value;

        private MessageId(ushort value)
        {
            this.value = value;
        }

        public static explicit operator MessageId(ushort value) => new MessageId(value);

        public static implicit operator ushort(MessageId messageId) => messageId.value;

        public static bool operator ==(MessageId left, MessageId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MessageId left, MessageId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> structure.
        /// </summary>
        /// <param name="ct">The <see cref="CancellationToken"/> that observe that asynchronous initialization of a new <see cref="MessageId"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous initialization of a new <see cref="MessageId"/>.
        /// The result of that <see cref="Task{TResult}"/> contains the new generated <see cref="MessageId"/>.</returns>
        /// <remarks>
        /// This method is thread-safe and uses a <see cref="SemaphoreSlim"/> to protect the access to the <see cref="System.Random"/> instance
        /// of the <see cref="MessageId"/>.
        /// </remarks>
        public static async Task<MessageId> NewMessageIdAsync(CancellationToken ct = default)
        {
            ushort value;

            await Mutex.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                value = (ushort)Random.Next(ushort.MinValue, ushort.MaxValue + 1);
            }
            finally
            {
                Mutex.Release();
            }

            var messageId = new MessageId(value);
            return messageId;
        }

        public bool Equals(MessageId other)
        {
            return this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is MessageId other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value.ToString("D");
        }
    }
}