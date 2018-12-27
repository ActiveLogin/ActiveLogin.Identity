using System;
using Xunit;
using ActiveLogin.Identity.Swedish.FSharp;

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
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(1999, 08, 07, 239, 1);
            Assert.Equal("199908072391", personalIdentityNumber.ToString());
        }

        [Fact]
        public void ToString_Returns_Native_FSharp_ToString()
        {
            var values = new FSharp.Types.SwedishPersonalIdentityNumberValues(1999, 08, 07, 239, 1);
            var personalIdentityNumber = FSharp.SwedishPersonalIdentityNumber.create(values).ResultValue;
            var expected = "{Year = Year 1999;\n" +
                           " Month = Month 8;\n" +
                           " Day = Day 7;\n" +
                           " BirthNumber = BirthNumber 239;\n" +
                           " Checksum = Checksum 1;}";
            Assert.Equal(expected, personalIdentityNumber.ToString());
        }
    }
}
