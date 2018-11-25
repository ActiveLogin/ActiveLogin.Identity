using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumberHintExtensions_GetDateOfBirthHint
    {
        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Year_Equals_Year(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            var dateOfBirth = personalIdentityNumber.GetDateOfBirthHint();
            Assert.Equal(year, dateOfBirth.Year);
        }

        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Month_Equals_Month(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            var dateOfBirth = personalIdentityNumber.GetDateOfBirthHint();
            Assert.Equal(month, dateOfBirth.Month);
        }

        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Day_Equals_Day(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            var dateOfBirth = personalIdentityNumber.GetDateOfBirthHint();
            Assert.Equal(day, dateOfBirth.Day);
        }
    }
}
