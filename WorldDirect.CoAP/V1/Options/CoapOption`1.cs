// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    /// <summary>
    /// Represents a generic, strong-typed option specified by RFC 7252.
    /// </summary>
    /// <typeparam name="TValue">The type of the value of that option.</typeparam>
    /// <seealso cref="WorldDirect.CoAP.V1.Options.CoapOption" />
    public abstract class CoapOption<TValue> : CoapOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption{TValue}"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="value">The value of that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="minLength">The minimum allowed length in bytes for that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="maxLength">The maximum allowed length in bytes for that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected CoapOption(ushort number, TValue value, uint minLength, uint maxLength, bool isRepeatable)
            : base(number, minLength, maxLength, isRepeatable)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapOption{TValue}"/> class.
        /// </summary>
        /// <param name="number">The number of that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="value">The value of that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="maxLength">The maximum allowed length in bytes for that <see cref="CoapOption{TValue}"/>.</param>
        /// <param name="isRepeatable">If set to <see langword="true"/> the <see cref="CoapOption"/> can be appear
        /// multiple times in a <see cref="OptionCollection"/>; Otherwise <see langword="false"/>.</param>
        protected CoapOption(ushort number, TValue value, uint maxLength, bool isRepeatable)
            : this(number, value, 0, maxLength, isRepeatable)
        {
        }

        /// <summary>
        /// Gets the strong typed value of that <see cref="CoapOption{TValue}"/> as <typeparamref name="TValue"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TValue Value { get; }

        /// <inheritdoc />
        public override string ToString() => $"{base.ToString()}: {this.Value}";
    }
}
