namespace WorldDirect.CoAP.Specs
{
    using Xunit;
    using FluentAssertions;

    public class VersionSpecs
    {
        /// <summary>
        /// The default version for RFC 7252 is binary value 01.
        /// </summary>
        [Fact]
        public void TheDefaultVersionForRfc7252IsBinaryValue01()
        {
            var defaultVersion = Version.DefaultRfc7252;
            ((UInt2)defaultVersion).Should().Be((UInt2)1);
        }
    }
}
