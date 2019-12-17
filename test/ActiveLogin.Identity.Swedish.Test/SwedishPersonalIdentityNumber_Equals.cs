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
        public void Two_Identical_PIN_When_One_Is_Object_Are_Equal_Using_Method()
        {
            var personalIdentityNumberString = "199908072391";
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var personalIdentityNumber2 = (object)SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
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
        public void Two_Identical_PIN_Are_Not_Unequal_Using_Operator()
        {
            var personalIdentityNumberString = "199908072391";
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var personalIdentityNumber2 = SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
            var notEquals = personalIdentityNumber1 != personalIdentityNumber2;

            Assert.False(notEquals);
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
        public void A_PIN_Is_Not_Equal_Null_Using_Method()
        {
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse("199908072391");
            var equals = personalIdentityNumber1.Equals((SwedishPersonalIdentityNumber)null);

            Assert.False(equals);
        }

        [Fact]
        public void A_PIN_Is_Not_Equal_Object_Null_Using_Method()
        {
            var personalIdentityNumber1 = SwedishPersonalIdentityNumber.Parse("199908072391");
            var equals = personalIdentityNumber1.Equals((object)null);

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

        [Fact]
        public void Two_Nulls_Are_Equal_Using_Operator()
        {
            Assert.True((SwedishPersonalIdentityNumber)null == (SwedishPersonalIdentityNumber)null);
        }

        [Fact]
        public void When_Left_Side_Is_Null_Should_Not_Equal_Using_Operator()
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse("191202119986");
            Assert.False((SwedishPersonalIdentityNumber)null == personalIdentityNumber);
        }

        [Fact]
        public void When_Right_Side_Is_Null_Should_Not_Equal_Using_Operator()
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse("191202119986");
            Assert.False(personalIdentityNumber == (SwedishPersonalIdentityNumber)null);
        }
    }
}
