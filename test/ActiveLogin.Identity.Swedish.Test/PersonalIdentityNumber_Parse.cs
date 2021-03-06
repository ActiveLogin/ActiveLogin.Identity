using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class PersonalIdentityNumber_Parse
    {
        private const string InvalidSwedishPersonalIdentityNumberErrorMessage = "String was not recognized as a valid IdentityNumber.";

        [Theory]
        [InlineData("900101+9802", 1890)]
        [InlineData("990913+9801", 1899)]
        [InlineData("120211+9986", 1912)]
        public void Parses_Year_From_10_Digit_String_When_Plus_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2012);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("900101+9802", 1890)]
        public void Parses_Year_From_10_Digit_String_When_Year_Is_Exact_100_Years(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 1990);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990807-2391", 1999)]
        [InlineData("180101-2392", 2018)]
        public void Parses_Year_From_10_Digit_String_When_Dash_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990913+9801", 1899)]
        [InlineData("120211+9986", 1912)]
        [InlineData("990807-2391", 1999)]
        [InlineData("180101-2392", 2018)]
        public void Parses_Year_From_10_Digit_String(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("990913+9801", 09)]
        [InlineData("120211+9986", 02)]
        [InlineData("990807-2391", 08)]
        [InlineData("180101-2392", 01)]
        public void Parses_Month_From_10_Digit_String(string personalIdentityNumberString, int expectedMonth)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedMonth, personalIdentityNumber.Month);
        }

        [Theory]
        [InlineData("990913+9801", 13)]
        [InlineData("120211+9986", 11)]
        [InlineData("990807-2391", 07)]
        [InlineData("180101-2392", 01)]
        public void Parses_Day_From_10_Digit_String(string personalIdentityNumberString, int expectedDay)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedDay, personalIdentityNumber.Day);
        }

        [Theory]
        [InlineData("990913+9801", 980)]
        [InlineData("120211+9986", 998)]
        [InlineData("990807-2391", 239)]
        [InlineData("180101-2392", 239)]
        public void Parses_BirthNumber_From_10_Digit_String(string personalIdentityNumberString, int expectedBirthNumber)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedBirthNumber, personalIdentityNumber.BirthNumber);
        }

        [Theory]
        [InlineData("990913+9801", 1)]
        [InlineData("120211+9986", 6)]
        [InlineData("990807-2391", 1)]
        [InlineData("180101-2392", 2)]
        public void Parses_Checksum_From_10_Digit_String(string personalIdentityNumberString, int expectedChecksum)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedChecksum, personalIdentityNumber.Checksum);
        }

        [Theory]
        [InlineData(" 990913+9801 ", "990913+9801")]
        [InlineData(" 990807-2391", "990807-2391")]
        [InlineData("180101-2392 ", "180101-2392")]
        public void Strips_Leading_And_Trailing_Whitespace_From_10_Digit_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To10DigitStringInSpecificYear(2018));
        }

        [Theory]
        [InlineData("189909139801", 1899)]
        [InlineData("191202119986", 1912)]
        public void Parses_Year_From_12_Digit_String_When_Plus_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("199908072391", 1999)]
        [InlineData("201801012392", 2018)]
        public void Parses_Year_From_12_Digit_String_When_Dash_Is_Delimiter(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("189909139801", 1899)]
        [InlineData("191202119986", 1912)]
        [InlineData("199908072391", 1999)]
        [InlineData("201801012392", 2018)]
        public void Parses_Year_From_12_Digit_String(string personalIdentityNumberString, int expectedYear)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedYear, personalIdentityNumber.Year);
        }

        [Theory]
        [InlineData("189909139801", 09)]
        [InlineData("191202119986", 02)]
        [InlineData("199908072391", 08)]
        [InlineData("201801012392", 01)]
        public void Parses_Month_From_12_Digit_String(string personalIdentityNumberString, int expectedMonth)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedMonth, personalIdentityNumber.Month);
        }

        [Theory]
        [InlineData("189909139801", 13)]
        [InlineData("191202119986", 11)]
        [InlineData("199908072391", 07)]
        [InlineData("201801012392", 01)]
        public void Parses_Day_From_12_Digit_String(string personalIdentityNumberString, int expectedDay)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedDay, personalIdentityNumber.Day);
        }

        [Theory]
        [InlineData("189909139801", 980)]
        [InlineData("191202119986", 998)]
        [InlineData("199908072391", 239)]
        [InlineData("201801012392", 239)]
        public void Parses_BirthNumber_From_12_Digit_String(string personalIdentityNumberString, int expectedBirthNumber)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedBirthNumber, personalIdentityNumber.BirthNumber);
        }

        [Theory]
        [InlineData("189909139801", 1)]
        [InlineData("191202119986", 6)]
        [InlineData("199908072391", 1)]
        [InlineData("201801012392", 2)]
        public void Parses_Checksum_From_12_Digit_String(string personalIdentityNumberString, int expectedChecksum)
        {
            var personalIdentityNumber = Parse(personalIdentityNumberString);
            Assert.Equal(expectedChecksum, personalIdentityNumber.Checksum);
        }

        [Theory]
        [InlineData(" 189909139801 ", "189909139801")]
        [InlineData(" 191202119986", "191202119986")]
        [InlineData("199908072391 ", "199908072391")]
        public void Strips_Leading_And_Trailing_Whitespace_From_12_Digit_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Theory]
        [InlineData("18990913-9801", "189909139801")]
        [InlineData("19120211-9986", "191202119986")]
        [InlineData("19990807-2391", "199908072391")]
        public void Parses_When_Hyphen_Delimiter_From_12_Digit_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Theory]
        [InlineData("18990913 9801", "189909139801")]
        [InlineData("19120211 9986", "191202119986")]
        [InlineData("19990807 2391", "199908072391")]
        public void Parses_When_Whitespace_Delimiter_From_12_Digit_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Theory]
        [InlineData("180101 2392", "201801012392")]
        [InlineData("990807 2391", "199908072391")]
        public void Parses_When_Whitespace_Delimiter_From_10_Digit_String(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Fact]
        public void Same_Number_Will_Use_Different_Delimiter_When_Parsed_On_Or_After_Person_Turns_100()
        {
            var withHyphen = "120211-9986";
            var withPlus = "120211+9986";

            var pinBeforeTurning100 = ParseInSpecificYear(withHyphen, 2011);
            var pinOnYearTurning100 = ParseInSpecificYear(withPlus, 2012);
            var pinAfterTurning100 = ParseInSpecificYear(withPlus, 2013);

            var expected = new PersonalIdentityNumber(1912, 02, 11, 998, 6);
            Assert.Equal(expected, pinBeforeTurning100);
            Assert.Equal(expected, pinOnYearTurning100);
            Assert.Equal(expected, pinAfterTurning100);
        }

        [Fact]
        public void Parses_When_Begins_With_Zero()
        {
            var personalIdentityNumber = Parse("000101-2384");
            Assert.Equal(new PersonalIdentityNumber(2000, 1, 1, 238, 4), personalIdentityNumber);
        }

        [Fact]
        public void Parses_When_Ends_With_Zero()
        {
            var personalIdentityNumber = Parse("170122-2380");
            Assert.Equal(new PersonalIdentityNumber(2017, 1, 22, 238, 0), personalIdentityNumber);
        }


        [Fact]
        public void Parse_Throws_FormatException_When_Empty_String()
        {
            var ex = Assert.Throws<FormatException>(() => Parse(""));

            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
            Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        }

        [Fact]
        public void Parse_Throws_ArgumentException_When_Whitespace_String()
        {
            var ex = Assert.Throws<FormatException>(() => Parse(" "));

            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
            Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        }

        [Fact]
        public void Parse_Throws_ArgumentNullException_When_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Parse(null));

            Assert.Contains("input", ex.Message);
        }

        [Fact]
        public void ParseInSpecificYear_Throws_FormatException_When_Empty_String()
        {
            var ex = Assert.Throws<FormatException>(() => ParseInSpecificYear("", 2018));

            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
            Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        }

        [Fact]
        public void ParseInSpecificYear_Throws_FormatException_When_Whitespace_String()
        {
            var ex = Assert.Throws<FormatException>(() => ParseInSpecificYear(" ", 2018));

            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
            Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        }

        [Fact]
        public void ParseInSpecificYear_Throws_ArgumentNullException_When_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParseInSpecificYear(null, 2018));

            Assert.Contains("input", ex.Message);
        }

        [Theory]
        [InlineData("99-09-13+980-1", "189909139801")]
        [InlineData("18-99-09-13-980-1", "189909139801")]
        [InlineData("99.09.13-+980.1", "189909139801")]
        [InlineData("18.99.09.13.980.1", "189909139801")]
        [InlineData("1899-09-13-980-1", "189909139801")]
        [InlineData("18 99 09 13 980 1", "189909139801")]
        [InlineData("18A99B09C13D980E1", "189909139801")]
        [InlineData("+18990913+9801+", "189909139801")]
        [InlineData("ABC189909139801ABC", "189909139801")]
        [InlineData("\"18\"\"99\"09\"13\"980\"1", "189909139801")]
        [InlineData("**18*99***09*13*980**1*", "189909139801")]
        [InlineData("\\18//99/;09\n13\t980\r1\n\r", "189909139801")]
        [InlineData("ü18ü99ù09ę13é980á1ö", "189909139801")]
        [InlineData("18----------------------------------------------------------------99-09-13-980-1", "189909139801")]
        [InlineData("18--DROP TABLE USERS; 99-09-13-980-1", "189909139801")]
        [InlineData("9909+13+9801", "189909139801")]
        [InlineData("189909+13+9801", "189909139801")]
        [InlineData("18+99+09+13+9801", "189909139801")]
        [InlineData("18+99+09+13+98+01", "189909139801")]
        [InlineData("1+8+9+9+0+9+1+3+9+8+0+1", "189909139801")]
        [InlineData("19990807+2391", "199908072391")]
        [InlineData("990807+2391", "189908072391")]
        [InlineData("990913�9801", "199909139801")]
        [InlineData("19990913�9801", "199909139801")]
        [InlineData("990913_9801", "199909139801")]
        [InlineData("19990913_9801", "199909139801")]
        [InlineData("990913.9801", "199909139801")]
        [InlineData("19990913.9801", "199909139801")]
        public void Parses_When_Contains_Chars(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        {
            var personalIdentityNumber = ParseInSpecificYear(personalIdentityNumberString, 2018);
            Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345678901")]
        [InlineData("1234567890123")]
        public void Throws_FormatException_When_Invalid_Number_Of_Digits(string personalIdentityNumberString)
        {
            var ex = Assert.Throws<FormatException>(() => ParseInSpecificYear(personalIdentityNumberString, 2018));
            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        }

        [Theory]
        // [InlineData("199913139801")]
        // [InlineData("199909139802")]
        [InlineData("199909329801")]
        public void Throws_FormatException_When_Invalid_Pin(string personalIdentityNumberString)
        {
            var ex = Assert.Throws<FormatException>(() => ParseInSpecificYear(personalIdentityNumberString, 2018));
            Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        }

        private PersonalIdentityNumber Parse(string input) => PersonalIdentityNumber.Parse(input, StrictMode.Off);
        private PersonalIdentityNumber ParseInSpecificYear(string input, int year) => PersonalIdentityNumber.ParseInSpecificYear(input, year, StrictMode.Off);
    }
}
