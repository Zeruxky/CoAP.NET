// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using WorldDirect.CoAP.V1.Options;

    /// <summary>
    /// Defines a factory for creating a <see cref="CoapOption"/> from a specified <see cref="OptionData"/>.
    /// </summary>
    public interface IOptionFactory
    {
        /// <summary>
        /// Gets the number of that <see cref="IOptionFactory"/>. The number indicates the number of the <see cref="CoapOption"/> that the factory can create.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        int Number { get; }

        /// <summary>
        /// Creates a <see cref="CoapOption"/> from the specified <see cref="OptionData"/>.
        /// </summary>
        /// <param name="src">The <see cref="OptionData"/> that holds the read data from the options reader.</param>
        /// <returns>The <see cref="CoapOption"/> that is constructed from the specified <see cref="OptionData"/>.</returns>
        CoapOption Create(OptionData src);
    }
}
