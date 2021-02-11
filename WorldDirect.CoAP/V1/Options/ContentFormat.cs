// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class ContentFormat : UIntOptionFormat
    {
        private const ushort MIN_VALUE = ushort.MinValue;
        private const ushort MAX_VALUE = ushort.MaxValue;

        protected ContentFormat(uint id, string mediaType, IEnumerable<Encoding> encodings)
            : base(id)
        {
            if (id > MAX_VALUE)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, $"The id of a content-format can only be in range of {MIN_VALUE} - {MAX_VALUE}.");
            }

            this.MediaType = mediaType;
            this.Encodings = new List<Encoding>(encodings);
        }

        protected ContentFormat(uint id, string mediaType)
            : this(id, mediaType, new List<Encoding>() { Encoding.UTF8 })
        {
        }

        public sealed override ushort Number => 12;

        public string MediaType { get; }

        public IReadOnlyList<Encoding> Encodings { get; }

        public uint Id => this.Value;

        public override string ToString()
        {
            return $"Content-Format ({this.Number}): {this.MediaType}";
        }
    }
}
