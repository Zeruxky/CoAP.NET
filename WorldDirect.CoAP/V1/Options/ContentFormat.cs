// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public abstract class ContentFormat : UIntOptionFormat
    {
        public const ushort NUMBER = 12;
        private const ushort MAX_LENGTH = 2;
        private const ushort MIN_LENGTH = 0;

        protected ContentFormat(uint id, string mediaType, IEnumerable<Encoding> encodings)
            : base(NUMBER, id, MAX_LENGTH, MIN_LENGTH)
        {
            this.MediaType = mediaType;
            this.Encodings = new List<Encoding>(encodings);
        }

        protected ContentFormat(uint id, string mediaType)
            : this(id, mediaType, Enumerable.Empty<Encoding>())
        {
        }

        public string MediaType { get; }

        public IReadOnlyList<Encoding> Encodings { get; }

        public uint Id => this.Value;

        public override string ToString() => $"{this.Name} ({this.Number}): {this.MediaType}";
    }
}
