using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_Equals
    {
        [Fact]
        public void Two_Identical_PIN_Are_Equal_Using_Method()
        {
            var personalIdentityNumberString = "199908072391";
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var personalIdentityNumber2 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var equals = personalIdentityNumber1.Equals(personalIdentityNumber2);

            Assert.True(equals);
        }

        [Fact]
        public void Two_Identical_PIN_Are_Equal_Using_Operator()
        {
            var personalIdentityNumberString = "199908072391";
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var personalIdentityNumber2 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var equals = personalIdentityNumber1 == personalIdentityNumber2;

            Assert.True(equals);
        }

        [Fact]
        public void Two_Different_PIN_Are_Not_Equal_Using_Method()
        {
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse("199908072391");
            var personalIdentityNumber2 = SwedishPersonalIdentityNumber.Parse("191202119986");
            var equals = personalIdentityNumber1.Equals(personalIdentityNumber2);

            Assert.False(equals);
        }

        [Fact]
        public void Two_Different_PIN_Are_Not_Equal_Using_Operator()
        {
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse("199908072391");
            var personalIdentityNumber2 = SwedishPersonalIdentityNumber.Parse("191202119986");
            var equals = personalIdentityNumber1 != personalIdentityNumber2;

            Assert.True(equals);
        }
    }
}
