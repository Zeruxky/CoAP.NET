// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a 8-bit unsigned integer and appears after the <see cref="Type"/> of the CoAP message.
    /// It is split into a 3-bit class (most significant bits) and a 5-bit detail (least significant bits).
    /// </summary>
    /// <remarks>
    /// The format of a <see cref="Code"/> is as follows: <c>c.dd</c> where <c>c</c> is a digit from 0 to 7
    /// for the 3-bit subfield and <c>dd</c> are two digits from 00 to 31 for the 5-bit subfield.
    ///
    /// Possible values are maintained at Section 12.1 in RFC 7252.
    /// </remarks>
    public class Code
    {
        private const byte CLASS_MASK = 0xE0;
        private const byte DETAIL_MASK = 0x07;
        private readonly byte value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="class">The <see cref="CodeClass"/> for this <see cref="Code"/>.</param>
        /// <param name="detail">The <see cref="CodeDetail"/> for this <see cref="Code"/>.</param>
        public Code(CodeClass @class, CodeDetail detail)
        {
            this.value = (byte)(((UInt3)@class << 5) | (UInt5)detail);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Code(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the subfield of the <see cref="Code"/> that represents the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public CodeClass Class => (CodeClass)(UInt3)((this.value & CLASS_MASK) >> 5);

        /// <summary>
        /// Gets the subfield of the <see cref="Code"/> that represents the detail.
        /// </summary>
        /// <value>
        /// The detail.
        /// </value>
        public CodeDetail Detail => (CodeDetail)(UInt5)(this.value & DETAIL_MASK);

        /// <summary>
        /// Performs an explicit conversion from <see cref="byte"/> to <see cref="Code"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The <see cref="Code"/> that is equivalent to the <see cref="byte"/> value.
        /// </returns>
        public static explicit operator Code(byte value) => new Code(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Code"/> to <see cref="byte"/>.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// The <see cref="byte"/> that is equivalent to the <paramref name="code"/>.
        /// </returns>
        public static implicit operator byte(Code code) => code.value;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Class}.{this.Detail}";
        }
    }

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
