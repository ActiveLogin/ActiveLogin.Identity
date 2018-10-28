using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_ToShortString
    {
        private readonly DateTime _date_2018_07_15 = new DateTime(2018, 07, 15);

        [Theory]
        [InlineData(1899, 09, 13, 980, 1, "990913+9801")]
        [InlineData(1912, 02, 11, 998, 6, "120211+9986")]
        public void When_Older_Than_100_Years_Uses_Plus_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToShortString(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Younger_Than_100_Years_Uses_Dash_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToShortString(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2000, 01, 02, 239, 1, "000102-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Date_Parts_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToShortString(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1990, 11, 16, 002, 6, "901116-0026")]
        public void When_birthNumber_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.ToShortString(_date_2018_07_15));
        }
    }
}
