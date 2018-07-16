using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_Birthdate
    {
        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Year_Equals_Year(int year, int month, int day, int serialNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            var birthdate = personalIdentityNumber.Birthdate;
            Assert.Equal(year, birthdate.Year);
        }

        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Month_Equals_Month(int year, int month, int day, int serialNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            var birthdate = personalIdentityNumber.Birthdate;
            Assert.Equal(month, birthdate.Month);
        }

        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1912, 02, 11, 998, 6)]
        public void Day_Equals_Day(int year, int month, int day, int serialNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, serialNumber, checksum);
            var birthdate = personalIdentityNumber.Birthdate;
            Assert.Equal(day, birthdate.Day);
        }
    }
}
