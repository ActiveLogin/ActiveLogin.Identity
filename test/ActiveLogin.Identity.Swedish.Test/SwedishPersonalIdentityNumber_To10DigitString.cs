using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_To10DigitString
    {
        [Theory]
        [InlineData(1890, 01, 01, 980, 2, "900101+9802")]
        [InlineData(1899, 09, 13, 980, 1, "990913+9801")]
        [InlineData(1912, 02, 11, 998, 6, "120211+9986")]
        public void The_Year_You_Turn_100_Years_Uses_Plus_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitStringInSpecificYear(2012));
        }

        [Theory]
        [InlineData(1890, 01, 01, 980, 2, "900101+9802")]
        public void The_Year_You_Turn_100_Years_Uses_Plus_As_Delimiter_Also_Exact_100_Years(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitStringInSpecificYear(1990));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Younger_Than_100_Years_Uses_Dash_As_Delimiter(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitStringInSpecificYear(2018));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, "990807-2391")]
        [InlineData(2000, 01, 02, 239, 1, "000102-2391")]
        [InlineData(2018, 01, 01, 239, 2, "180101-2392")]
        public void When_Date_Parts_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitStringInSpecificYear(2018));
        }

        [Theory]
        [InlineData(1990, 11, 16, 002, 6, "901116-0026")]
        public void When_birthNumber_Has_Leading_Zeroes_String_Has_Leading_Zeroes(int year, int month, int day, int birthNumber, int checksum, string expected)
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expected, personalIdentityNumber.To10DigitStringInSpecificYear(2018));
        }

        [Fact]
        public void NumberString_Will_Use_Different_Delimiter_When_Executed_On_Or_After_Person_Turns_100()
        {
            var pin = new SwedishPersonalIdentityNumber(1912, 02, 11, 998, 6);

            var stringBeforeTurning100 = pin.To10DigitStringInSpecificYear(2011);
            var stringOnYearTurning100 = pin.To10DigitStringInSpecificYear(2012);
            var stringAfterTurning100 = pin.To10DigitStringInSpecificYear(2013);

            var withHyphen = "120211-9986";
            var withPlus = "120211+9986";
            Assert.Equal(withHyphen, stringBeforeTurning100);
            Assert.Equal(withPlus, stringOnYearTurning100);
            Assert.Equal(withPlus, stringAfterTurning100);
        }

        [Fact]
        public void Serializes_Beginning_Zeroes()
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(2000, 1, 1, 238, 4);
            Assert.Equal("000101-2384", personalIdentityNumber.To10DigitString());
        }

        [Fact]
        public void Serializes_Ending_Zeroes()
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(2017, 1, 22, 238, 0);
            Assert.Equal("170122-2380", personalIdentityNumber.To10DigitString());
        }

        [Fact]
        public void Thows_When_Invalid_Year()
        {
            var personalIdentityNumber = new SwedishPersonalIdentityNumber(2017, 1, 22, 238, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => personalIdentityNumber.To10DigitStringInSpecificYear(-1));
        }

        [Theory]
        [InlineData("120211+9986", 1912)]
        [InlineData("990807-2391", 1999)]
        public void To10DigitString_WhenParseYearIs200YearsAfterPersonWasBorn_ThrowsArgumentException(string str, int birthYear)
        {
            var serializationYear = birthYear + 200;
            var pin = SwedishPersonalIdentityNumber.Parse(str);
            Assert.Throws<ArgumentOutOfRangeException>(() => pin.To10DigitStringInSpecificYear(serializationYear ));
        }

        [Theory]
        [InlineData("120211+9986", 1912)]
        [InlineData("990807-2391", 1999)]
        public void To10DigitString_WhenParseYearIsBeforePersonIsBorn_ThrowsArgumentException(string str, int birthYear)
        {
            var serializationYear = birthYear - 1;
            var pin = SwedishPersonalIdentityNumber.Parse(str);
            Assert.Throws<ArgumentOutOfRangeException>(() => pin.To10DigitStringInSpecificYear(serializationYear));
        }
    }
}
