using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <summary>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </summary>
    public class SwedishPersonalIdentityNumber_Gender
    {
        [Theory]
        [InlineData(1899, 09, 13, 980, 1, 118)]
        [InlineData(1912, 02, 11, 998, 6, 106)]

        public void When_Last_Digit_In_SerialNumber_Is_Even_It_Is_A_Woman(int year, int month, int day, int serialNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, serialNumber, checksum);
            Assert.Equal(SwedishLegalGender.Woman, personalIdentityNumber.LegalGender);
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, 18)]
        [InlineData(2018, 01, 01, 239, 2, 0)]
        public void When_Last_Digit_In_SerialNumber_Is_Odd_It_Is_A_Man(int year, int month, int day, int serialNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, serialNumber, checksum);
            Assert.Equal(SwedishLegalGender.Man, personalIdentityNumber.LegalGender);
        }
    }
}
