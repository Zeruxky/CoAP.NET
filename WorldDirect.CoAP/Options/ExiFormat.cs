﻿namespace WorldDirect.CoAP.Options
{
    public sealed class ExiFormat : ContentFormat
    {
        public ExiFormat()
            : base(47)
        {
        }

        public override string MediaType => "application/exi";
    }
}