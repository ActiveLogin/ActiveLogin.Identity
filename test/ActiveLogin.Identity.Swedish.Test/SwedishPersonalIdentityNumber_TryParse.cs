using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
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
            Assert.Equal(SwedishPersonalIdentityNumber.Create(2000, 01, 01, 238, 4), personalIdentityNumber);
        }
    }
}
