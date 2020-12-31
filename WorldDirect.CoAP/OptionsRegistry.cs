namespace WorldDirect.CoAP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionsRegistry
    {
        private readonly IDictionary<ushort, Func<byte[], IOption>> registry;

        public OptionsRegistry()
        {
            this.registry = new Dictionary<ushort, Func<byte[], IOption>>();
        }

        public void Add(ushort number, Func<byte[], IOption> factory)
        {
            this.registry.Add(number, factory);
        }

        public IOption CreateOption(ushort number, byte[] value)
        {
            var factory = this.registry.Single(o => o.Key.Equals(number)).Value;
            return factory(value);
        }
    }
}
