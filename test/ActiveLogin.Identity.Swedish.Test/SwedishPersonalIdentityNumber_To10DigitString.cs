using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_To10DigitString
    {
        private readonly DateTime _date_2018_07_15 = new DateTime(2018, 07, 15);
        private readonly DateTime _date_2012_01_01 = new DateTime(2012, 01, 01);

        [Theory]
        [InlineData(1890, 01, 01, 980, 2, "900101+9802")]
        [InlineData(1899, 09, 13, 980, 1, "990913+9801")]
        [InlineData(1912, 02, 11, 998, 6, "120211+9986")]
        public void The_Year_You_Turn_100_Years_Uses_Plus_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitString(_date_2012_01_01));
        }

        [Theory]
        [InlineData(1890, 01, 01, 980, 2, "900101+9802")]
        public void The_Year_You_Turn_100_Years_Uses_Plus_As_Delimiter_Also_Exact_100_Years(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitString(new DateTime(1990, 01, 01)));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Younger_Than_100_Years_Uses_Dash_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitString(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2000, 01, 02, 239, 1, "000102-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Date_Parts_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitString(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1990, 11, 16, 002, 6, "901116-0026")]
        public void When_birthNumber_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitString(_date_2018_07_15));
        }

        [Fact]
        public void NumberString_Will_Use_Different_Delimiter_When_Executed_On_Or_After_Person_Turns_100()
        {
            var pin = SwedishPersonalIdentityNumber.Create(1912, 02, 11, 998, 6);

            var stringBeforeTurning100 = pin.To10DigitString(new DateTime(2011, 1, 1));
            var stringOnYearTurning100 = pin.To10DigitString(new DateTime(2012, 1, 1));
            var stringAfterTurning100 = pin.To10DigitString(new DateTime(2013, 1, 1));

            var withHyphen = "120211-9986";
            var withPlus = "120211+9986";
            Assert.Equal(withHyphen, stringBeforeTurning100);
            Assert.Equal(withPlus, stringOnYearTurning100);
            Assert.Equal(withPlus, stringAfterTurning100);
        }
    }
}
