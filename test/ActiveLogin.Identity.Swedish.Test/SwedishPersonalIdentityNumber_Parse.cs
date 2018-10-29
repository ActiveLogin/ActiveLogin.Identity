using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_Parse
    {
        private const string InvalidSwedishPersonalIdentityNumberErrorMessage = "Invalid Swedish personal identity number.";
        private readonly DateTime _date_2018_07_15 = new DateTime(2018, 07, 15);

        [Theory]
        [InlineData("990913+9801", 1899)]
        [InlineData("120211+9986", 1912)]
        public void Parses_Year_From_Short_String_When_Plus_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990807-2391", 1999)]
        [InlineData("180101-2392", 2018)]
        public void Parses_Year_From_Short_String_When_Dash_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990913+9801", 1899)]
        [InlineData("120211+9986", 1912)]
        [InlineData("990807-2391", 1999)]
        [InlineData("180101-2392", 2018)]
        public void Parses_Year_From_Short_String(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990913+9801", 09)]
        [InlineData("120211+9986", 02)]
        [InlineData("990807-2391", 08)]
        [InlineData("180101-2392", 01)]
        public void Parses_Month_From_Short_String(string personalIdentityNumberString, int expectedMonth)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedMonth, personalIdentityNumber.Month);
        }

        [Theory]
        [InlineData("990913+9801", 13)]
        [InlineData("120211+9986", 11)]
        [InlineData("990807-2391", 07)]
        [InlineData("180101-2392", 01)]
        public void Parses_Day_From_Short_String(string personalIdentityNumberString, int expectedDay)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedDay, personalIdentityNumber.Day);
        }

        [Theory]
        [InlineData("990913+9801", 980)]
        [InlineData("120211+9986", 998)]
        [InlineData("990807-2391", 239)]
        [InlineData("180101-2392", 239)]
        public void Parses_BirthNumber_From_Short_String(string personalIdentityNumberString, int expectedBirthNumber)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedBirthNumber, personalIdentityNumber.BirthNumber);
        }

        [Theory]
        [InlineData("990913+9801", 1)]
        [InlineData("120211+9986", 6)]
        [InlineData("990807-2391", 1)]
        [InlineData("180101-2392", 2)]
        public void Parses_Checksum_From_Short_String(string personalIdentityNumberString, int expectedChecksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedChecksum, personalIdentityNumber.Checksum);
        }

        [Theory]
        [InlineData(" 990913+9801 ", "990913+9801")]
        [InlineData(" 990807-2391", "990807-2391")]
        [InlineData("180101-2392 ", "180101-2392")]
        public void Strips_Leading_And_Trailing_Whitespace_From_Short_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To10DigitString(_date_2018_07_15));
        }

        [Theory]
        [InlineData("990913 9801")]
        [InlineData("990913—9801")]
        [InlineData("990913_9801")]
        [InlineData("990913.9801")]
        public void Throws_When_Invalid_Delimiter_From_Short_String(string personalIdentityNumberString)
        {
            var ex = Assert.Throws<ArgumentException>(() => SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15));
            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        }



        [Theory]
        [InlineData("189909139801", 1899)]
        [InlineData("191202119986", 1912)]
        public void Parses_Year_From_Long_String_When_Plus_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("199908072391", 1999)]
        [InlineData("201801012392", 2018)]
        public void Parses_Year_From_Long_String_When_Dash_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("189909139801", 1899)]
        [InlineData("191202119986", 1912)]
        [InlineData("199908072391", 1999)]
        [InlineData("201801012392", 2018)]
        public void Parses_Year_From_Long_String(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("189909139801", 09)]
        [InlineData("191202119986", 02)]
        [InlineData("199908072391", 08)]
        [InlineData("201801012392", 01)]
        public void Parses_Month_From_Long_String(string personalIdentityNumberString, int expectedMonth)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedMonth, personalIdentityNumber.Month);
        }

        [Theory]
        [InlineData("189909139801", 13)]
        [InlineData("191202119986", 11)]
        [InlineData("199908072391", 07)]
        [InlineData("201801012392", 01)]
        public void Parses_Day_From_Long_String(string personalIdentityNumberString, int expectedDay)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedDay, personalIdentityNumber.Day);
        }

        [Theory]
        [InlineData("189909139801", 980)]
        [InlineData("191202119986", 998)]
        [InlineData("199908072391", 239)]
        [InlineData("201801012392", 239)]
        public void Parses_BirthNumber_From_Long_String(string personalIdentityNumberString, int expectedBirthNumber)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedBirthNumber, personalIdentityNumber.BirthNumber);
        }

        [Theory]
        [InlineData("189909139801", 1)]
        [InlineData("191202119986", 6)]
        [InlineData("199908072391", 1)]
        [InlineData("201801012392", 2)]
        public void Parses_Checksum_From_Long_String(string personalIdentityNumberString, int expectedChecksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            Assert.Equal(expectedChecksum, personalIdentityNumber.Checksum);
        }

        [Theory]
        [InlineData(" 189909139801 ", "189909139801")]
        [InlineData(" 191202119986", "191202119986")]
        [InlineData("199908072391 ", "199908072391")]
        public void Strips_Leading_And_Trailing_Whitespace_From_Long_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Theory]
        [InlineData("18990913+9801")]
        public void Throws_When_Plus_Delimiter_From_Long_String(string personalIdentityNumberString)
        {
            var ex = Assert.Throws<ArgumentException>(() => SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15));
            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        }

        [Theory]
        [InlineData("18990913 9801")]
        [InlineData("990913—9801")]
        [InlineData("990913_9801")]
        [InlineData("990913.9801")]
        public void Throws_When_Invalid_Delimiter_From_Long_String(string personalIdentityNumberString)
        {
            var ex = Assert.Throws<ArgumentException>(() => SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString, _date_2018_07_15));
            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        }
    }
}
