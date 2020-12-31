namespace WorldDirect.CoAP.Options
{
    using System;
    using System.Text;

    public class ContentFormat : UIntOptionFormat, IContentFormat
    {
        public static readonly ContentFormat TextPlain = new TextPlainContentFormat();
        public static readonly ContentFormat LinkFormat = new LinkFormat();
        public static readonly ContentFormat XmlFormat = new XmlFormat();
        public static readonly ContentFormat OctetStreamFormat = new OctetStreamFormat();
        public static readonly ContentFormat ExiFormat = new ExiFormat();
        public static readonly ContentFormat Json = new JsonFormat();

        public ContentFormat(uint value)
            : base(value)
        {
            if (value > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value for Content-Format can only be in range of {ushort.MinValue} - {ushort.MaxValue}.");
            }
        }

        public sealed override ushort Number => 12;

        public virtual string MediaType { get; }

        public virtual Encoding Encoding => Encoding.UTF8;

        public uint Id => this.Value;

        public override string ToString()
        {
            return $"Content-Format ({this.Number}): {this.MediaType}";
        }
    }
}
