// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;
    using WorldDirect.CoAP.Messages;

    /// <summary>
    /// Represents the result of the deserialization of a <see cref="CoapMessage"/>.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.IDeserializerResult{CoapMessage}" />
    public class MessageDeserializerResult : DeserializerResult<CoapMessage>
    {
        private MessageDeserializerResult(CoapMessage message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Creates a successful <see cref="MessageDeserializerResult"/> from the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The <see cref="CoapMessage"/>.</param>
        /// <returns>A successful <see cref="MessageDeserializerResult"/> with the given <paramref name="message"/> as its value.</returns>
        public static MessageDeserializerResult FromMessage(CoapMessage message) => new MessageDeserializerResult(message, null);

        /// <summary>
        /// Creates a failed <see cref="MessageDeserializerResult"/> from the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that caused the <see cref="MessageDeserializerResult"/> to be failed.</param>
        /// <returns>A failed <see cref="MessageDeserializerResult"/> with the given <paramref name="exception"/> as its causing <see cref="Exception"/>.</returns>
        public static MessageDeserializerResult FromException(Exception exception) => new MessageDeserializerResult(null, exception);
    }
}
