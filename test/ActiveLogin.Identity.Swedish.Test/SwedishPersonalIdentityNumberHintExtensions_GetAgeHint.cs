using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumberHintExtensions_GetAgeHint
    {
        private readonly DateTime _date_2018_07_15 = new DateTime(2018, 07, 15);
        private readonly DateTime _date_2000_04_14 = new DateTime(2000, 04, 14);

        [Theory]
        [InlineData(1899, 09, 13, 980, 1, 118)]
        [InlineData(1912, 02, 11, 998, 6, 106)]
        public void When_Older_Than_100_Years_Calculates_Age(int year, int month, int day, int birthNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expectedAge, personalIdentityNumber.GetAgeHint(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, 18)]
        [InlineData(2018, 01, 01, 239, 2, 0)]
        public void When_Younger_Than_100_Years_Calculates_Age(int year, int month, int day, int birthNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(expectedAge, personalIdentityNumber.GetAgeHint(_date_2018_07_15));
        }

        [Theory]
        [InlineData(2018, 01, 01, 239, 2)]
        public void When_Not_Yet_Born_Throws_Exception(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            var ex = Assert.Throws<Exception>(() => personalIdentityNumber.GetAgeHint(_date_2000_04_14));

            Assert.Contains("The person is not yet born.", ex.Message);
        }
    }
}
