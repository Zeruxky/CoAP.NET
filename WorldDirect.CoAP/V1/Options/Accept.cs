// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;

    public class Accept : UIntOptionFormat
    {
        public const ushort Id = 17;

        public Accept(uint value)
            : base(Id, value, 0, 2)
        {
        }
    }

    public class AcceptFactory : IOptionFactory
    {
        public CoapOption Create(OptionData src)
        {
            return new Accept(src.UIntValue);
        }

        public int Number => Accept.Id;
    }
}
