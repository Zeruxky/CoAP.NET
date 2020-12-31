namespace WorldDirect.CoAP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionsFactory
    {
        private readonly IDictionary<ushort, Func<byte[], IOption>> values;

        public OptionsFactory()
        {
            this.values = new Dictionary<ushort, Func<byte[], IOption>>();
        }

        public void Add(ushort number, Func<byte[], IOption> constructor)
        {
            this.values.Add(number, constructor);
        }

        public void Add(KeyValuePair<ushort, Func<byte[], IOption>> value)
        {
            this.values.Add(value);
        }

        public IOption CreateOption(ushort number, byte[] value)
        {
            var constructor = this.values.Single(o => o.Key.Equals(number)).Value;
            return constructor(value);
        }
    }
}
