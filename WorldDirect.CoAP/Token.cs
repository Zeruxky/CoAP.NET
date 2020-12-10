namespace WorldDirect.CoAP
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public struct Token : IEquatable<Token>
    {
        /// <summary>
        /// The <see cref="TokenLength"/> of that <see cref="Token"/>.
        /// </summary>
        public readonly TokenLength Length;

        // Protects the access to the Random field.
        private static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        private static readonly Random Random = new Random();
        private readonly ulong value;

        private Token(ulong value, TokenLength length)
        {
            this.Length = length;
            this.value = value;
        }

        public static implicit operator ulong(Token token) => token.value;

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Token"/> structure.
        /// </summary>
        /// <param name="length">The <see cref="TokenLength"/> that indicates the maximum value of the <see cref="Token"/>.</param>
        /// <param name="ct">The <see cref="CancellationToken"/> that observes the asynchronous initialization of a new <see cref="Token"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous initialization of a new <see cref="Token"/>.
        /// The result of that <see cref="Task{TResult}"/> is the new created <see cref="Token"/>.</returns>
        public static async Task<Token> NewTokenAsync(TokenLength length, CancellationToken ct = default)
        {
            ulong value;
            await Mutex.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                value = Random.NextULong((UInt4)length);
            }
            finally
            {
                Mutex.Release();
            }

            var token = new Token(value, length);
            return token;
        }

        public bool Equals(Token other)
        {
            return Equals(this.Length, other.Length) && this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Token other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Length, this.value);
        }
    }
}