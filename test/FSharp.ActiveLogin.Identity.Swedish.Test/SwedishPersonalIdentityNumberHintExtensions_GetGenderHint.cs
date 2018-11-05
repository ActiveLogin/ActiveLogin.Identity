using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumberHintExtensions_GetGenderHint
    {
        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]

        public void When_Last_Digit_In_birthNumber_Is_Even_It_Is_A_Woman(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(Gender.Female, personalIdentityNumber.GetGenderHint());
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1)]
        [InlineData(2018, 01, 01, 239, 2)]
        public void When_Last_Digit_In_birthNumber_Is_Odd_It_Is_A_Man(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(Gender.Male, personalIdentityNumber.GetGenderHint());
        }
    }
}