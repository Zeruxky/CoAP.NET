// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using WorldDirect.CoAP.Messages;

    /// <summary>
    /// Defines the result of a deserialization with output type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the resulting value.</typeparam>
    public interface IDeserializerResult<out TValue>
    {
        /// <summary>
        /// Gets the exception that caused the deserialization to be failed/stopped.
        /// </summary>
        /// <value>
        /// The causing exception.
        /// </value>
        Exception CausingException { get; }

        /// <summary>
        /// Gets the resulting <see cref="CoapMessage"/>.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        TValue Value { get; }

        /// <summary>
        /// Gets a value indicating whether the deserialization has been failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        bool Failed { get; }

        int Position { get; set; }
    }
}
