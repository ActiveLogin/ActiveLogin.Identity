using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class PersonalIdentityNumber_HashCode
    {
        [Fact]
        public void Two_Identical_PIN_Returns_Same_Hashcode()
        {
            var personalIdentityNumberString = "199908072391";
            var personalIdentityNumber1 = PersonalIdentityNumber.Parse(personalIdentityNumberString);
            var personalIdentityNumber2 = PersonalIdentityNumber.Parse(personalIdentityNumberString);

            Assert.Equal(personalIdentityNumber1.GetHashCode(), personalIdentityNumber2.GetHashCode());
        }
    }
}
