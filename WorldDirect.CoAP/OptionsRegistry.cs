namespace WorldDirect.CoAP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionsRegistry
    {
        private readonly List<IOption> options;

        public OptionsRegistry()
        {
            this.options = new List<IOption>();
        }

        public OptionsRegistry(IEnumerable<IOption> options)
        {
            this.options = new List<IOption>(options);
        }

        public void Add(IOption option)
        {
            this.options.Add(option);
        }

        public IOption GetOption(ushort number)
        {
            var option = this.options.Single(o => o.Number.Equals(number));
            return option;
        }
    }
}
