// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Specs
{
    using System.Text;
    using V1.Options;
    using WorldDirect.CoAP.V1;
    using Xunit;

    public class UriHostFactorySpec
    {
        private readonly UriHostFactory cut;

        public UriHostFactorySpec()
        {
            this.cut = new UriHostFactory();
        }

        [Fact]
        public void X()
        {
            var content = Encoding.UTF8.GetBytes("Hällo");
            var optionData = new OptionData(0, 1, (ushort)content.Length, content);
            var option = this.cut.Create(optionData);
        }
    }
}
