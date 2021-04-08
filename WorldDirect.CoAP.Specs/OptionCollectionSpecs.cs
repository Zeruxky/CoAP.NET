namespace WorldDirect.CoAP.Specs
{
    using FluentAssertions;
    using V1;
    using V1.Options;
    using Xunit;

    public class OptionCollectionSpecs
    {
        private readonly OptionCollection cut;

        public OptionCollectionSpecs()
        {
            this.cut = new OptionCollection();
        }

        [Fact]
        public void TheCollectionCanAddAOption()
        {
            var expectedOption = new UriPath("1234");
            this.cut.Add(expectedOption);
            this.cut.Contains(expectedOption).Should().BeTrue();
        }

        [Fact]
        public void TheCollectionCanRemoveAOption()
        {
            var option = new UriPath("1234");
            var collection = new OptionCollection()
            {
                option,
            };

            collection.Remove(option);
            collection.Contains(option).Should().BeFalse();
        }
    }
}