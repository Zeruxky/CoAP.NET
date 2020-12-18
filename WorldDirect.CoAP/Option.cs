namespace WorldDirect.CoAP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Reflection;

    public abstract class Enumeration : IComparable
    {
        protected Enumeration(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Name { get; private set; }

        public int Id { get; private set; }

        public override string ToString() => this.Name;

        public static IEnumerable<T> GetAll<T>()
            where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration otherValue))
            {
                return false;
            }

            var typeMatches = this.GetType() == obj.GetType();
            var valueMatches = this.Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => this.Id.CompareTo(((Enumeration)other).Id);
    }

    public interface IOption
    {
        int Number { get; }

        OptionType Type { get; }

        string Name { get; }

        OptionFormat Format { get; }

        int MinValue { get; }

        int MaxValue { get; }
    }

    public interface IOption<TIn> : IOption
    {
        IOption Default(TIn value);
    }

    public class IfMatchOption : IOption
    {
        private IfMatchOption()
        {
        }

        public int Number => 1;

        public OptionType Type => OptionType.Critical | OptionType.Repeatable;

        public string Name => "If-Match";

        public OptionFormat Format => OptionFormat.Opaque;

        public int MinValue => 0;

        public int MaxValue => 8;
    }

    public class UriHost : IOption<string>
    {
        private UriHost()
        {
        }

        public int Number => 1;

        public OptionType Type => OptionType.Critical | OptionType.Repeatable;

        public string Name => "If-Match";

        public OptionFormat Format => OptionFormat.Opaque;

        public int MinValue => 0;

        public int MaxValue => 8;

        public IOption Default(string value)
        {
            throw new NotImplementedException();
        }
    }

    public class OptionLength
    {
        public OptionLength(int min, int max)
        {
            this.MinValue = min;
            this.MaxValue = max;
        }

        public int MinValue { get; }

        public int MaxValue { get; }
    }

    public enum OptionFormat
    {
        Empty,

        Opaque,

        Uint,

        String,
    }

    [Flags]
    public enum OptionType
    {
        Critical = 0,

        Unsafe = 1,

        NoCacheKey = 2,

        Repeatable = 4,
    }

    public class Option
    {
        public Option(ushort delta, ushort length, byte[] value)
        {
            if (value.Length != length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, $"Expected {length} bytes (Found {value.Length} bytes).");
            }

            this.Value = value;
            this.Delta = delta;
            this.Length = length;
        }

        public ushort Delta { get; }

        public ushort Length { get; }

        public IReadOnlyCollection<byte> Value { get; }
    }

    public interface IDeserializer<in TIn, out TOut>
    {
        bool CanDeserialize(Version version);

        TOut Deserialize(TIn value);
    }

    public class OptionV1Deserializer : IDeserializer<byte[], Option>
    {
        private const byte MASK_LENGTH = 0x0F;
        private const byte MASK_DELTA = 0xF0;

        public bool CanDeserialize(Version version)
        {
            return version.Equals(Version.V1);
        }

        public Option Deserialize(byte[] value)
        {
            if (value[0] == 0xFF)
            {
                throw new MessageFormatError("Detected payload marker. Not allowed in Option.");
            }

            var delta = (ushort)((value[0] & MASK_DELTA) >> 4);
            var startOfLength = 1;
            if (delta > 12)
            {
                if (delta == 13)
                {
                    delta = (ushort)(value[1] - 13);
                }

                if (delta == 14)
                {
                    delta = (ushort)(BitConverter.ToUInt16(value, 1) - 269);
                    startOfLength = 2;
                }
            }

            var length = (ushort)(value[0] & MASK_LENGTH);
            var endOfLength = startOfLength + 1;
            if (length > 12)
            {
                if (length == 13)
                {
                    length = (ushort)(value[startOfLength] - 13);
                }

                if (length == 14)
                {
                    length = (ushort)(BitConverter.ToUInt16(value, startOfLength) - 269);
                    endOfLength = startOfLength + 2;
                }

                if (length == 15)
                {
                    throw new MessageFormatError("Length 15 (0xFF) is reserved for future use.");
                }
            }

            var optionValue = value.Slice(endOfLength);
            return new Option(delta, length, optionValue);
        }
    }

    public static class ByteArrayExtensions
    {
        public static byte[] Slice(this byte[] value, int start)
        {
            return value.Slice(start, value.Length);
        }

        public static byte[] Slice(this byte[] value, int start, int end)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, "Start index is not allowed to be negative.");
            }

            if (end < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(end), end, "End index is not allowed to be negative.");
            }

            if (start > value.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, $"Start index must be in the range of 0 - {value.Length}.");
            }

            if (end > value.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(end), end, $"End index must be in the range of {start} - {value.Length}.");
            }

            if (start > end)
            {
                throw new InvalidOperationException("Start index is not allowed to be greater than end index.");
            }

            var array = new byte[end - start];
            for (var i = start; i < end; i++)
            {
                array[i] = value[i];
            }

            return array;
        }
    }
}
