namespace WorldDirect.CoAP.Stack
{
    using Net;

    /// <summary>
    /// Represent a next layer in the stack.
    /// </summary>
    public interface INextLayer
    {
        /// <summary>
        /// Sends a request to next layer.
        /// </summary>
        void SendRequest(Exchange exchange, Request request);
        /// <summary>
        /// Sends a response to next layer.
        /// </summary>
        void SendResponse(Exchange exchange, Response response);
        /// <summary>
        /// Sends an empty message to next layer.
        /// </summary>
        void SendEmptyMessage(Exchange exchange, EmptyMessage message);
        /// <summary>
        /// Receives a request to next layer.
        /// </summary>
        void ReceiveRequest(Exchange exchange, Request request);
        /// <summary>
        /// Receives a response to next layer.
        /// </summary>
        void ReceiveResponse(Exchange exchange, Response response);
        /// <summary>
        /// Receives an empty message to next layer.
        /// </summary>
        void ReceiveEmptyMessage(Exchange exchange, EmptyMessage message);
    }
}