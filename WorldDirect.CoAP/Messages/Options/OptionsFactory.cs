namespace WorldDirect.CoAP.Messages.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class OptionsFactory
    {
        public OptionsFactory()
        {
            this.Options = new Dictionary<ushort, Func<byte[], ICoapOption>>()
            {
                { 1, value => new IfMatch(value) },
                { 3, value => new UriHost(Encoding.UTF8.GetString(value)) },
                { 4, value => new ETag(value) },
                { 5, value => new IfNoneMatch() },
                { 7, value => new UriPort(BitConverter.ToUInt16(value, 0)) },
                { 8, value => new LocationPath(Encoding.UTF8.GetString(value)) },
                { 11, value => new UriPath(Encoding.UTF8.GetString(value)) },
                { 12, value => new ContentFormat(BitConverter.ToUInt16(value, 0)) },
                { 14, value => new MaxAge(BitConverter.ToUInt16(value, 0)) },
                { 15, value => new UriQuery(Encoding.UTF8.GetString(value)) },
                { 17, value => new Accept(BitConverter.ToUInt16(value, 0)) },
                { 20, value => new LocationQuery(Encoding.UTF8.GetString(value)) },
                { 35, value => new ProxyUri(Encoding.UTF8.GetString(value)) },
                { 39, value => new ProxyScheme(Encoding.UTF8.GetString(value)) },
                { 60, value => new Size1(BitConverter.ToUInt16(value, 0)) },
            };
        }

        public OptionsFactory(IDictionary<ushort, Func<byte[], ICoapOption>> values)
        {
            this.Options = values;
        }

        public IDictionary<ushort, Func<byte[], ICoapOption>> Options { get; }

        public ICoapOption CreateOption(ushort number, Span<byte> value)
        {
            var constructor = this.Options.Single(o => o.Key.Equals(number)).Value;
            return constructor(value.ToArray());
        }
    }
}
