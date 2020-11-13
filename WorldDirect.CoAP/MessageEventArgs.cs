namespace WorldDirect.CoAP
{
    using System;

    /// <summary>
    /// Represents an event of a message.
    /// </summary>
    /// <typeparam name="T">the type of the message</typeparam>
    public class MessageEventArgs<T> : EventArgs where T : Message
    {
        readonly T _message;

        /// <summary>
        /// 
        /// </summary>
        public MessageEventArgs(T message)
        {
            _message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public T Message
        {
            get { return _message; }
        }
    }
}