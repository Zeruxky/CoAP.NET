namespace WorldDirect.CoAP.Specs
{
    using System;
    using Common.Extensions;
    using FluentAssertions;
    using V1.Options;
    using Xunit;

    public class UriHostSpecs
    {
        [Fact]
        public void UriHostOnlyAccepts255Bytes()
        {
            const string hexString = "be bf 87 3b 16 8c 00 06 05 f8 cb cd 40 6b 67 36 be 76 d8 84 0d b4 " +
                                     "f8 14 4b c3 29 8c 2c a3 fb b7 2f 69 7f b3 95 b3 4b fb ad 91 96 7e " +
                                     "0f 5a d7 84 98 43 96 2a 9d d8 51 0a c7 73 f0 52 63 74 d9 62 fc 14 " +
                                     "9e d6 ec b4 a0 0b 2a dc d6 7b 1d 79 04 f0 ec a4 f3 e9 0c d0 19 56 " +
                                     "17 75 ec 22 c2 f9 98 39 b7 33 81 fb bc 00 98 e4 a3 8b ff c1 02 d6 " +
                                     "ec 4d 6f ff 66 bf 80 f0 56 2c fb 47 bf 77 c7 7d 48 31 43 9d c3 ea " +
                                     "f7 09 34 eb 0b 0f 45 80 d0 57 53 41 88 9f b3 c3 92 d9 b8 c7 dd 55 " +
                                     "00 74 41 21 1c 25 43 ce 7d 65 12 02 55 66 ca 7b b3 71 ab d5 5b a3 " +
                                     "f4 7d ee 28 ca 84 9f 6a e5 fc 0c a1 08 c0 a9 ea f2 1f 3d 02 da ee " +
                                     "2f 86 49 4d f9 17 26 46 1c 08 fa 5c 80 c3 32 f4 5b be 2e 13 80 15 " +
                                     "f5 b3 5d 09 d1 c8 7f 8b 98 58 c9 05 4e 67 de 7d 8b 87 44 9e e0 04 " +
                                     "a7 ab 40 e5 b7 fd c4 7e d3 c6 6e 19 91 95";
            var bytes = ByteArrayExtensions.FromHexString(hexString);
            Assert.Throws<ArgumentOutOfRangeException>(() => new UriHost(bytes));
        }

        [Fact]
        public void UriHostNumberIsThree()
        {
            UriHost.NUMBER.Should().Be(3);
        }
    }
}