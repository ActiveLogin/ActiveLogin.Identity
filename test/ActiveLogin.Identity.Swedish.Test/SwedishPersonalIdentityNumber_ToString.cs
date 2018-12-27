using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_ToString
    {
        [Fact]
        public void ToString_Returns_12DigitString()
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(1999, 08, 07, 239, 1);
            Assert.Equal("199908072391", personalIdentityNumber.ToString());
        }
    }
}
