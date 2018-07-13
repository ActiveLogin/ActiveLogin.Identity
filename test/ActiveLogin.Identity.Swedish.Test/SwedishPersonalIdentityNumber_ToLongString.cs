using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_ToLongString
    {
        [Theory]
        [InlineData(1912, 02, 11, 998, 6, "191202119986")]
        [InlineData(1899, 09, 13, 980, 1, "189909139801")]
        public void When_Older_Than_100_Years_Uses_No_Delimiter_But_Four_Digit_Year(int year, int month, int day, int serialNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToLongString());
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "199908072391")]
        [InlineData(2018, 01, 01, 239, 2, "201801012392")]
        public void When_Younger_Than_100_Years_Uses_No_Delimiter_But_Four_Digit_Year(int year, int month, int day, int serialNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToLongString());
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "199908072391")]
        [InlineData(2000, 01, 02, 239, 1, "200001022391")]
        [InlineData(2018, 01, 01, 239, 2, "201801012392")]
        public void When_Date_Parts_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int serialNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToLongString());
        }

        [Theory]
        [InlineData(1990, 11, 16, 002, 6, "199011160026")]
        public void When_SerialNumber_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int serialNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToLongString());
        }
    }
}
