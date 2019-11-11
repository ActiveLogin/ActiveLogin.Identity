using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_ToString
    {
        [Fact]
        public void ToString_Returns_12DigitString()
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(1999, 08, 07, 239, 1);
            Assert.Equal("199908072391", personalIdentityNumber.ToString());
        }

        [Fact]
        public void ToString_Returns_Native_FSharp_ToString()
        {
            var personalIdentityNumber =
                FSharp.SwedishPersonalIdentityNumber.create(1999, 08, 07, 239, 1).ResultValue;
            var str = personalIdentityNumber.ToString();
            Assert.Contains("Year 1999", str);
            Assert.Contains("Month 8", str);
            Assert.Contains("Day 7", str);
            Assert.Contains("BirthNumber 239", str);
            Assert.Contains("Checksum 1", str);
        }
    }
}
