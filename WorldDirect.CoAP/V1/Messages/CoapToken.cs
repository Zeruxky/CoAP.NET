// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Messages
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using WorldDirect.CoAP.Common;
    using WorldDirect.CoAP.Common.Extensions;

    public readonly struct CoapToken : IEquatable<CoapToken>
    {
        public static readonly CoapToken EmptyToken = new CoapToken(0, new CoapTokenLength((UInt4)0));

        /// <summary>
        /// The <see cref="CoapTokenLength"/> of that <see cref="CoapToken"/>.
        /// </summary>
        public readonly CoapTokenLength Length;

        private static readonly Random Random = new Random();
        private readonly ulong value;

        public CoapToken(ulong value, CoapTokenLength length)
        {
            this.Length = length;
            this.value = value;
        }

        public static implicit operator ulong(CoapToken token) => token.value;

        public static explicit operator CoapToken(ulong value) => new CoapToken(value, (CoapTokenLength)(UInt4)BitConverter.GetBytes(value).Length);

        public static bool operator ==(CoapToken left, CoapToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoapToken left, CoapToken right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CoapToken"/> structure.
        /// </summary>
        /// <param name="length">The <see cref="CoapTokenLength"/> that indicates the maximum value of the <see cref="CoapToken"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous initialization of a new <see cref="CoapToken"/>.
        /// The result of that <see cref="Task{TResult}"/> is the new created <see cref="CoapToken"/>.</returns>
        public static CoapToken NewToken(CoapTokenLength length)
        {
            var value = Random.NextULong((UInt4)length);
            var token = new CoapToken(value, length);
            return token;
        }

        public bool Equals(CoapToken other)
        {
            return this.Length.Equals(other.Length) && this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CoapToken)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Length, this.value);
        }

        public override string ToString()
        {
            return this.value.ToString($"x{(UInt4)this.Length * 2}");
        }
    }
}
