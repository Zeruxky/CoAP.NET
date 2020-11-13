namespace WorldDirect.CoAP
{
    /// <summary>
    /// Response status codes.
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 2.01 Created
        /// </summary>
        Created = 65,
        /// <summary>
        /// 2.02 Deleted
        /// </summary>
        Deleted = 66,
        /// <summary>
        /// 2.03 Valid 
        /// </summary>
        Valid = 67,
        /// <summary>
        /// 2.04 Changed
        /// </summary>
        Changed = 68,
        /// <summary>
        /// 2.05 Value
        /// </summary>
        Content = 69,
        /// <summary>
        /// 2.?? Continue
        /// </summary>
        Continue = 95,
        /// <summary>
        /// 4.00 Bad Request
        /// </summary>
        BadRequest = 128,
        /// <summary>
        /// 4.01 Unauthorized
        /// </summary>
        Unauthorized = 129,
        /// <summary>
        /// 4.02 Bad Option
        /// </summary>
        BadOption = 130,
        /// <summary>
        /// 4.03 Forbidden
        /// </summary>
        Forbidden = 131,
        /// <summary>
        /// 4.04 Not Found
        /// </summary>
        NotFound = 132,
        /// <summary>
        /// 4.05 Method Not Allowed
        /// </summary>
        MethodNotAllowed = 133,
        /// <summary>
        /// 4.06 Not Acceptable
        /// </summary>
        NotAcceptable = 134,
        /// <summary>
        /// 4.08 Request Entity Incomplete (draft-ietf-core-block)
        /// </summary>
        RequestEntityIncomplete = 136,
        /// <summary>
        /// 
        /// </summary>
        PreconditionFailed = 140,
        /// <summary>
        /// 4.13 Request Entity Too Large
        /// </summary>
        RequestEntityTooLarge = 141,
        /// <summary>
        /// 4.15 Unsupported Media Type
        /// </summary>
        UnsupportedMediaType = 143,
        /// <summary>
        /// 5.00 Internal Server Error
        /// </summary>
        InternalServerError = 160,
        /// <summary>
        /// 5.01 Not Implemented
        /// </summary>
        NotImplemented = 161,
        /// <summary>
        /// 5.02 Bad Gateway
        /// </summary>
        BadGateway = 162,
        /// <summary>
        /// 5.03 Service Unavailable 
        /// </summary>
        ServiceUnavailable = 163,
        /// <summary>
        /// 5.04 Gateway Timeout
        /// </summary>
        GatewayTimeout = 164,
        /// <summary>
        /// 5.05 Proxying Not Supported
        /// </summary>
        ProxyingNotSupported = 165
    }
}