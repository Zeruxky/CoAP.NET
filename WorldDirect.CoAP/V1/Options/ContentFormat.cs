// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class StringExtensions
    {
        /// <summary>
        /// Insert dashes into the specified <see cref="string"/> where a uppercase letter is. This means that the string "HelloWorld" will be constructed to "Hello-World".
        /// </summary>
        /// <param name="value">The <see cref="string"/> that should be transformed.</param>
        /// <returns>A newly created <seealso cref="string"/> that contains dashes where a uppercase letter is.</returns>
        public static string Dasherize(this string value)
        {
            var builder = new StringBuilder();

            builder.Append(value[0]);
            for (int i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    builder.Append('-');
                }

                builder.Append(value[i]);
            }

            return builder.ToString();
        }

        public static string Dasherize(this CoapOption option)
        {
            if (option.GetType() == typeof(ETag))
            {
                return "ETag";
            }

            return option.GetType().Name.Dasherize();
        }
    }

    public class ContentFormatFactory : IOptionFactory
    {
        private readonly ContentFormatRegistry registry;

        public ContentFormatFactory(ContentFormatRegistry registry)
        {
            this.registry = registry;
        }

        public CoapOption Create(OptionData src)
        {
            var id = src.UIntValue;
            return this.registry.Get(c => c.Id.Equals(id));
        }

        public int Number => ContentFormat.NUMBER;
    }

    public abstract class ContentFormat : UIntOptionFormat
    {
        public const ushort NUMBER = 12;

        protected ContentFormat(uint id, string mediaType, IEnumerable<Encoding> encodings)
            : base(NUMBER, id, 0, 2)
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
