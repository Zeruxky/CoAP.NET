﻿namespace WorldDirect.CoAP.Options
{
    using System.Text;

    public interface IContentFormat
    {
        string MediaType { get; }

        Encoding Encoding { get; }

        uint Id { get; }
    }
}