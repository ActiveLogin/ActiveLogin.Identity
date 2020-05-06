using System;
using System.Globalization;
using Xunit;
using ActiveLogin.Identity.Swedish.Extensions;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with official test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class PersonalIdentityNumberHintExtensions_GetAgeHint
    {
        private readonly DateTime _date_2018_07_15 = new DateTime(2018, 07, 15);
        private readonly DateTime _date_2000_04_14 = new DateTime(2000, 04, 14);

        [Theory]
        [InlineData(1899, 09, 13, 980, 1, 118)]
        [InlineData(1912, 02, 11, 998, 6, 106)]
        public void When_Older_Than_100_Years_Calculates_Age(int year, int month, int day, int birthNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = new PersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expectedAge, personalIdentityNumber.GetAgeHint(_date_2018_07_15));
        }

        [Theory]
        [InlineData(1999, 08, 07, 239, 1, 18)]
        [InlineData(2018, 01, 01, 239, 2, 0)]
        public void When_Younger_Than_100_Years_Calculates_Age(int year, int month, int day, int birthNumber, int checksum, int expectedAge)
        {
            var personalIdentityNumber = new PersonalIdentityNumber(year, month, day, birthNumber, checksum);
            Assert.Equal(expectedAge, personalIdentityNumber.GetAgeHint(_date_2018_07_15));
        }

        [Theory]
        [InlineData(2018, 01, 01, 239, 2)]
        public void When_Not_Yet_Born_Throws_Exception(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = new PersonalIdentityNumber(year, month, day, birthNumber, checksum);
            var ex = Assert.Throws<ArgumentException>(() => personalIdentityNumber.GetAgeHint(_date_2000_04_14));

            Assert.Contains("The person is not yet born.", ex.Message);
        }


        [Fact]
        public void Without_Date_Uses_UtcNow()
        {
            var personalIdentityNumber = new PersonalIdentityNumber(1899, 09, 13, 980, 1);
            Assert.Equal(personalIdentityNumber.GetAgeHint(DateTime.UtcNow), personalIdentityNumber.GetAgeHint());
        }

        [Theory]
        // Birth non leap year, before "leap day"
        [InlineData("201701052399", "20180104", 0)]
        [InlineData("201701052399", "20180105", 1)]
        [InlineData("201701052399", "20190104", 1)]
        [InlineData("201701052399", "20190105", 2)]
        [InlineData("201701052399", "20200104", 2)]
        [InlineData("201701052399", "20200105", 3)]

        // Birth non leap year, after "leap day"
        [InlineData("201703052397", "20180304", 0)]
        [InlineData("201703052397", "20180305", 1)]
        [InlineData("201703052397", "20190304", 1)]
        [InlineData("201703052397", "20190305", 2)]
        [InlineData("201703052397", "20200304", 2)]
        [InlineData("201703052397", "20200305", 3)]

        // Birth leap year, after leap day
        [InlineData("201603102383", "20170309", 0)]
        [InlineData("201603102383", "20170310", 1)]
        [InlineData("201603102383", "20180309", 1)]
        [InlineData("201603102383", "20180310", 2)]
        [InlineData("201603102383", "20190309", 2)]
        [InlineData("201603102383", "20190310", 3)]
        [InlineData("201603102383", "20200309", 3)]
        [InlineData("201603102383", "20200310", 4)]

        // Birth leap year, on leap day
        [InlineData("201602292383", "20160229", 0)]
        [InlineData("201602292383", "20170228", 0)]
        [InlineData("201602292383", "20170301", 1)]
        [InlineData("201602292383", "20200228", 3)]
        [InlineData("201602292383", "20200229", 4)]
        public void GetAgeHint_Handles_LeapYears_Correctly(string personalIdentityNumber, string actualDate, int expectedAge)
        {
            // Arrange
            var swedishPersonalIdentityNumber = PersonalIdentityNumber.Parse(personalIdentityNumber);
            var date = DateTime.ParseExact(actualDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

            // Act
            var age = swedishPersonalIdentityNumber.GetAgeHint(date);

            // Assert
            Assert.Equal(expectedAge, age);
        }
    }
}
