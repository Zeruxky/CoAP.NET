// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Specs
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using WorldDirect.CoAP.Common.Extensions;
    using WorldDirect.CoAP.V1.Options;
    using Xunit;

    public class IfMatchSpecs
    {
        [Fact]
        public void IfMatchOnlyAcceptsEightBytes()
        {
            const string hexString = "be bf 87 3b 16 8c 00 06 05";
            var bytes = ByteArrayExtensions.FromHexString(hexString).ToArray();
            Assert.Throws<ArgumentOutOfRangeException>(() => new IfMatch(bytes));
        }

        [Fact]
        public void IfMatchNumberIsOne()
        {
            IfMatch.NUMBER.Should().Be(1);
        }

        [Fact]
        public void IfMatchIsCritical()
        {
            const string hexString = "be bf 87 3b 16 8c 00 06";
            var bytes = ByteArrayExtensions.FromHexString(hexString).ToArray();
            var ifMatchOption = new IfMatch(bytes);
            ifMatchOption.IsCritical.Should().BeTrue();
        }
    }
}
