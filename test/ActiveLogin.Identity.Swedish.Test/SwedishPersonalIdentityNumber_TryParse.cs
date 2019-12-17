using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_TryParse
    {
        [Fact]
        public void TryParse_Returns_False_And_Outputs_Null_When_Invalid_Day()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse("170199-2388", out SwedishPersonalIdentityNumber personalIdentityNumber);

            Assert.False(isValid);
            Assert.Null(personalIdentityNumber);
        }

        [Fact]
        public void TryParse_Returns_True_And_Outs_PIN_When_Valid_PIN()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse("000101-2384", out SwedishPersonalIdentityNumber personalIdentityNumber);

            Assert.True(isValid);
            Assert.Equal(new SwedishPersonalIdentityNumber(2000, 01, 01, 238, 4), personalIdentityNumber);
        }

        [Fact]
        public void TryParse_Returns_False_When_Valid_CoordinationNumber()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse("170182-2387", out SwedishPersonalIdentityNumber _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Returns_True_And_Outs_PIN_When_Valid_PIN()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParseInSpecificYear("990913+9801", 2018, out SwedishPersonalIdentityNumber personalIdentityNumber);

            Assert.True(isValid);
            Assert.Equal(new SwedishPersonalIdentityNumber(1899, 09, 13, 980, 1), personalIdentityNumber);
        }

        [Fact]
        public void TryParseInSpecificYear_Returns_False_When_Invalid_Year()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParseInSpecificYear("990913+9801", -1, out SwedishPersonalIdentityNumber _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Empty_String()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse("", out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Whitespace_String()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse(" ", out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParse_Return_False_When_Null()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParse(null, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Empty_String()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParseInSpecificYear("", 2018, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Whitespace_String()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParseInSpecificYear(" ", 2018, out _);

            Assert.False(isValid);
        }

        [Fact]
        public void TryParseInSpecificYear_Return_False_When_Null()
        {
            var isValid = SwedishPersonalIdentityNumber.TryParseInSpecificYear(null, 2018, out _);

            Assert.False(isValid);
        }
    }
}
