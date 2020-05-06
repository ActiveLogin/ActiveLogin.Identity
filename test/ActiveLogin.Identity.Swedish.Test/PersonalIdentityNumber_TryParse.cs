using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class PersonalIdentityNumber_TryParse
    {
        [Fact]
        public void TryParse_Returns_False_And_Outputs_Null_When_Invalid_Day()
        {
            var isValid = PersonalIdentityNumber.TryParse("170199-2388", out PersonalIdentityNumber personalIdentityNumber);

            Assert.False(isValid);
            Assert.Null(personalIdentityNumber);
        }

        [Fact]
        public void TryParse_Returns_True_And_Outs_PIN_When_Valid_PIN()
        {
            var isValid = PersonalIdentityNumber.TryParse("000101-2384", out PersonalIdentityNumber personalIdentityNumber);

            Assert.True(isValid);
            Assert.Equal(new PersonalIdentityNumber(2000, 01, 01, 238, 4), personalIdentityNumber);
        }

        [Fact]
        public void TryParse_Returns_False_When_Valid_CoordinationNumber()
        {
            var isValid = PersonalIdentityNumber.TryParse("170182-2387", out PersonalIdentityNumber _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Returns_True_And_Outs_PIN_When_Valid_PIN()
        {
            var isValid = PersonalIdentityNumber.TryParseInSpecificYear("990913+9801", 2018, out PersonalIdentityNumber personalIdentityNumber);

            Assert.True(isValid);
            Assert.Equal(new PersonalIdentityNumber(1899, 09, 13, 980, 1), personalIdentityNumber);
        }

        [Fact]
        public void TryParseInSpecificYear_Returns_False_When_Invalid_Year()
        {
            var isValid = PersonalIdentityNumber.TryParseInSpecificYear("990913+9801", -1, out PersonalIdentityNumber _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Empty_String()
        {
            var isValid = PersonalIdentityNumber.TryParse("", out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Whitespace_String()
        {
            var isValid = PersonalIdentityNumber.TryParse(" ", out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Null()
        {
            var isValid = PersonalIdentityNumber.TryParse(null, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Empty_String()
        {
            var isValid = PersonalIdentityNumber.TryParseInSpecificYear("", 2018, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Whitespace_String()
        {
            var isValid = PersonalIdentityNumber.TryParseInSpecificYear(" ", 2018, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Null()
        {
            var isValid = PersonalIdentityNumber.TryParseInSpecificYear(null, 2018, out _);

            Assert.False(isValid);
        }
    }
}
